﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Labrys.Generation
{
    /// <summary>
    /// A Dictionary mapping Vector2Int positions to Section objects.
    /// </summary>
    public class Grid
    {
        private List<Feature> features;

        // TODO: Dictionary gets slow for large sets (>10k). Implement a faster hashmap
        // implementation, or just have buckets of a max size? Something to improve speed.
        private Dictionary<Vector2Int, Section> globalGrid;

        // How many distinct cells are set in this grid
        public int NumFullCells 
        {
            get; private set;
        }

        private HashSet<Vector2Int> boundary;

        public Grid()
        {
            features = new List<Feature>();
            globalGrid = new Dictionary<Vector2Int, Section>();
            boundary = new HashSet<Vector2Int>();
        }

        // Vector2Int access
        public Section? this[Vector2Int pos]
        {
            get
            {
                if (globalGrid.TryGetValue(pos, out Section val))
                {
                    return val;
                }
                return null;
            }

            set
            {
                // Prevent nulls from being added in
                if (value == null)
                {
                    return;
                }

                if (globalGrid.ContainsKey(pos))
                {
                    //Debug.LogWarning($"Overriding existing Section at {pos}.");
                    globalGrid[pos] = (Section)value;
                }
                else
                {
                    globalGrid.Add(pos, (Section)value);

                    // Add to count
                    NumFullCells++;

                    // Update boundary
                    bool IsInternal(Vector2Int position) 
                    {
                        return this[position + Vector2Int.up] != null &&
                            this[position + Vector2Int.right] != null &&
                            this[position + Vector2Int.down] != null &&
                            this[position + Vector2Int.left] != null;
                    }

                    // If this new position is internal, add it to boundary
                    if (!IsInternal(pos) && !boundary.Contains(pos)) 
                    {
                        boundary.Add(pos);
                    }

                    // Check all neighbors. Remove if they're now internal.
                    if (boundary.Contains(pos + Vector2Int.up) && IsInternal(pos + Vector2Int.up)) 
                    {
                        boundary.Remove(pos + Vector2Int.up);
                    }

                    if (boundary.Contains(pos + Vector2Int.down) && IsInternal(pos + Vector2Int.down))
                    {
                        boundary.Remove(pos + Vector2Int.down);
                    }

                    if (boundary.Contains(pos + Vector2Int.left) && IsInternal(pos + Vector2Int.left))
                    {
                        boundary.Remove(pos + Vector2Int.left);
                    }

                    if (boundary.Contains(pos + Vector2Int.right) && IsInternal(pos + Vector2Int.right))
                    {
                        boundary.Remove(pos + Vector2Int.right);
                    }

                }
            }
        }

        // Separate access
        public Section? this[int x, int y]
        {
            get => this[new Vector2Int(x, y)];
            set => this[new Vector2Int(x, y)] = value;
        }

        public Vector2Int[] GetOccupiedCells()
        {
            Vector2Int[] toReturn = new Vector2Int[globalGrid.Count];
            globalGrid.Keys.CopyTo(toReturn, 0);
            return toReturn;
        }

        public Vector2Int[] GetBoundary()
        {
            Vector2Int[] toReturn = new Vector2Int[boundary.Count];
            boundary.CopyTo(toReturn);
            return toReturn;
        }

        /// <summary>
        /// If a Section has a neighbor, sets a Connection flag. Returns the combined
        /// set of Connection flags.
        /// 
        /// Note: This is the easily readable, but much slower version of the function.
        /// This function will likely be deprecated at some point.
        /// 
        /// </summary>
        /// <returns>The physical adjacencies.</returns>
        /// <param name="position">Position.</param>
        public Connection GetPhysicalAdjacencies_OLD(Vector2Int position)
        {
            Connection physicalAdjacent = Connection.None;

            Section thisSection = globalGrid[position];

            /*
             * Cardinal directions are connected iff (using North as example):
             * 
             * thisSection is allowed to connect North
             * there is a Section to the North
             * the North Section is allowed to connect South
             */

            /*
             * Diagonal connections are connected iff (using NE as example):
             * 
             * thisSection is allowed to connect North, East, and Northeast
             * there is a Section to the North, East, and Northeast
             * the North Section is allowed to connect South, East, and Southeast
             * the East Section is allowed to connect West, North, and Northwest
             * the Northeast Section is allowed to connect West, South, and Southwest
             * 
             * Since connections and adjacencies are 1:1 if allowed, we're good.
             */

            // North
            if (thisSection.CanConnect(Connection.North))
            {
                if (globalGrid.TryGetValue(position + Vector2Int.up, out Section up))
                {
                    if (up.CanConnect(Connection.South))
                    {
                        physicalAdjacent |= Connection.North;
                    }
                }
            }

            // East
            if (thisSection.CanConnect(Connection.East))
            {
                if (globalGrid.TryGetValue(position + Vector2Int.right, out Section right))
                {
                    if (right.CanConnect(Connection.West))
                    {
                        physicalAdjacent |= Connection.East;
                    }
                }
            }

            // South
            if (thisSection.CanConnect(Connection.South))
            {
                if (globalGrid.TryGetValue(position + Vector2Int.down, out Section down))
                {
                    if (down.CanConnect(Connection.North))
                    {
                        physicalAdjacent |= Connection.South;
                    }
                }
            }

            // West
            if (thisSection.CanConnect(Connection.West))
            {
                if (globalGrid.TryGetValue(position + Vector2Int.left, out Section left))
                {
                    if (left.CanConnect(Connection.East))
                    {
                        physicalAdjacent |= Connection.West;
                    }
                }
            }

            // NW
            if (thisSection.CanConnect(Connection.West | Connection.North | Connection.Northwest))
            {
                bool okay = true;

                if (globalGrid.TryGetValue(position + Vector2Int.up, out Section up))
                {
                    okay &= up.CanConnect(Connection.West | Connection.South | Connection.Southwest);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.left, out Section left))
                {
                    okay &= left.CanConnect(Connection.North | Connection.East | Connection.Northeast);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.up + Vector2Int.left, out Section upLeft))
                {
                    okay &= upLeft.CanConnect(Connection.East | Connection.South | Connection.Southeast);
                }
                else
                {
                    okay = false;
                }

                physicalAdjacent |= okay ? Connection.Northwest : Connection.None;
            }

            // NE
            if (thisSection.CanConnect(Connection.North | Connection.East | Connection.Northeast))
            {
                bool okay = true;

                if (globalGrid.TryGetValue(position + Vector2Int.up, out Section up))
                {
                    okay &= up.CanConnect(Connection.East | Connection.South | Connection.Southeast);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.right, out Section right))
                {
                    okay &= right.CanConnect(Connection.North | Connection.West | Connection.Northwest);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.up + Vector2Int.right, out Section upRight))
                {
                    okay &= upRight.CanConnect(Connection.West | Connection.South | Connection.Southwest);
                }
                else
                {
                    okay = false;
                }

                physicalAdjacent |= okay ? Connection.Northeast : Connection.None;
            }

            // SW
            if (thisSection.CanConnect(Connection.West | Connection.South | Connection.Southwest))
            {
                bool okay = true;

                if (globalGrid.TryGetValue(position + Vector2Int.down, out Section down))
                {
                    okay &= down.CanConnect(Connection.West | Connection.North | Connection.Northwest);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.left, out Section left))
                {
                    okay &= left.CanConnect(Connection.South | Connection.East | Connection.Southeast);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.down + Vector2Int.left, out Section downLeft))
                {
                    okay &= downLeft.CanConnect(Connection.East | Connection.North | Connection.Northeast);
                }
                else
                {
                    okay = false;
                }

                physicalAdjacent |= okay ? Connection.Southwest : Connection.None;
            }

            // SE
            if (thisSection.CanConnect(Connection.South | Connection.East | Connection.Southeast))
            {
                bool okay = true;

                if (globalGrid.TryGetValue(position + Vector2Int.down, out Section down))
                {
                    okay &= down.CanConnect(Connection.East | Connection.North | Connection.Northeast);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.right, out Section right))
                {
                    okay &= right.CanConnect(Connection.South | Connection.West | Connection.Southwest);
                }
                else
                {
                    okay = false;
                }

                if (globalGrid.TryGetValue(position + Vector2Int.down + Vector2Int.right, out Section downRight))
                {
                    okay &= downRight.CanConnect(Connection.West | Connection.North | Connection.Northwest);
                }
                else
                {
                    okay = false;
                }

                physicalAdjacent |= okay ? Connection.Southeast : Connection.None;
            }

            return physicalAdjacent;
        }

        // The adjacent positions, in order
        private static Vector2Int[] adjacentOffsets =
        {
            Vector2Int.right,
            Vector2Int.right + Vector2Int.up,
            Vector2Int.up,
            Vector2Int.left + Vector2Int.up,
            Vector2Int.left,
            Vector2Int.left + Vector2Int.down,
            Vector2Int.down,
            Vector2Int.right + Vector2Int.down
        };

        // All the connections, in counterclockwise order.
        // More are added to prevent the need for modulo operators.
        private static Connection[] connectionsOrdered =
        {
            Connection.East,
            Connection.Northeast,
            Connection.North,
            Connection.Northwest,
            Connection.West,
            Connection.Southwest,
            Connection.South,
            Connection.Southeast,

            Connection.East, // 8
            Connection.Northeast,
            Connection.North,
            Connection.Northwest // 11
        };

        // Static reusable storage for the 8 adjacent sections.
        private static Section?[] adjacentSections = new Section?[8];

        // The cache works best when you have many connections, and iterate over sections in a way that
        // lets you see all neighbors of an element before the element is removed from the cache.
        // 
        // E.g., if you have a dense grid of size NxN, and you iterate over it in row major order, 
        // then a cache size of 3N + 3 will ensure an element gets accessed the maximum of 8 times in the cache.
        // 
        // Dense dungeons w/ many connections benefit best from block iteration, with block size MxM where
        // M^2 is less than the slowdown speed of Dictionaries, which is ~10k. 
        //
        // Sparser duneons benefit from traversing down dungeon branches. BFS and DFS traversal will individually
        // be bad for the cache: BFS will spread so wide the old elements in the cache don't get seen, while DFS
        // will add many elements to the cache going down a path that only get used once. Some combination is ideal,
        // based on the specific branching structure of the dungeon.
        private GridSectionCache sectionCache = new GridSectionCache(5000);

        // Static reusable storage for the current position + the grid offset.
        private static Vector2Int positionPlusOffset;

        // Static reusable storage for a 2x2 subsection of a 3x3 adjacency grid.
        // Used to determine if a diagonal connection is allowed. 
        private static Section?[] TwoByTwoArea = new Section?[4];

        // The connections checked by Check2x2, in order.
        private static readonly Connection[] connectionsToCheck =
        {
            Connection.East,
            Connection.Northeast,
            Connection.North,
            Connection.North,
            Connection.Northwest,
            Connection.West,
            Connection.West,
            Connection.Southwest,
            Connection.South,
            Connection.South,
            Connection.Southeast,
            Connection.East
        };

        /// <summary>
        /// Gets the physical adjacencies at a given position.
        /// Specifically optimized for speed vs. "GetPhysicalAdjacencies".
        /// </summary>
        /// <returns>The physical adjacencies.</returns>
        /// <param name="position">Position.</param>
        public Connection GetPhysicalAdjacencies(Vector2Int position)
        {
            Connection physicalAdjacent2 = Connection.None;

            Section thisSection = globalGrid[position];

            Profiler.BeginSample("Basic checks");
            // Build up a cache, do basic checks (sufficient for cardinal directions)
            for (int i = 0; i < 8; i++)
            {
                if (thisSection.CanConnect(connectionsOrdered[i]))
                {
                    // Cache which one of the neighboring positions we're looking at.
                    positionPlusOffset = position + adjacentOffsets[i];

                    // The Section at the neighboring position
                    Section? foundSection = null;

                    // Try to find the section in the cache first.
                    Profiler.BeginSample("Cache lookup");
                    foundSection = sectionCache.Get(positionPlusOffset);
                    Profiler.EndSample();

                    // If we found it - report. Can remove this when profiling is over.
                    if (foundSection != null)
                    {
                        Profiler.BeginSample("Cache hit");
                        Profiler.EndSample();
                    }
                    // Didn't find it- try looking it up in the big dictionary.
                    else
                    {
                        Profiler.BeginSample("Cache miss - TryGetValue");
                        if (globalGrid.TryGetValue(positionPlusOffset, out Section globalFoundSection))
                        {
                            // Ensure it's in the cache for next time.
                            sectionCache.Add(positionPlusOffset, globalFoundSection);
                            foundSection = globalFoundSection;
                        }
                        Profiler.EndSample();
                    }

                    // Still not found in the big dictionary. Doesn't exist, so skip it.
                    if (foundSection == null)
                    {
                        Profiler.BeginSample("Section not found skip");
                        Profiler.EndSample();
                        continue;
                    }

                    // Guaranteed to be found and not null at this point; Section? -> Section cast
                    adjacentSections[i] = (Section)foundSection;

                    Profiler.BeginSample("Section found postprocessing");
                    // Ensure the neighboring section can connect back to the original position.
                    // i + 4 is the opposite direction connection; e.g., N => S.
                    if (((Section)adjacentSections[i]).CanConnect(connectionsOrdered[i + 4]))
                    {
                        physicalAdjacent2 |= connectionsOrdered[i];
                    }
                    Profiler.EndSample();


                }
            }
            Profiler.EndSample();
            Profiler.BeginSample("Diagonal checks");

            /*
             * Check the diagonal connections. If they're false, then don't compute further-
             * if they're true, then we need to do additional checks. Specifically, we only
             * checked if there is something in the diagonal direction that we can connect
             * to, and that it can connect back. We also need to see if the adjacent cardinal
             * connections exist and can connect.
             * 
             * Check2x2 expects sections in this orientation:
             * 
             * 3-2
             * 0-1
             * 
             * And returns true they are fully connected (i.e., following the diagonal rules).
             */

            if ((physicalAdjacent2 & Connection.Northeast) == Connection.Northeast)
            {
                TwoByTwoArea[0] = thisSection;
                TwoByTwoArea[1] = adjacentSections[0];
                TwoByTwoArea[2] = adjacentSections[1];
                TwoByTwoArea[3] = adjacentSections[2];

                if (!Check2x2(TwoByTwoArea))
                {
                    physicalAdjacent2 &= ~Connection.Northeast;
                }
            }

            if ((physicalAdjacent2 & Connection.Northwest) == Connection.Northwest)
            {
                TwoByTwoArea[0] = adjacentSections[4];
                TwoByTwoArea[1] = thisSection;
                TwoByTwoArea[2] = adjacentSections[2];
                TwoByTwoArea[3] = adjacentSections[3];

                if (!Check2x2(TwoByTwoArea))
                {
                    physicalAdjacent2 &= ~Connection.Northwest;
                }
            }

            if ((physicalAdjacent2 & Connection.Southwest) == Connection.Southwest)
            {
                TwoByTwoArea[0] = adjacentSections[5];
                TwoByTwoArea[1] = adjacentSections[6];
                TwoByTwoArea[2] = thisSection;
                TwoByTwoArea[3] = adjacentSections[4];

                if (!Check2x2(TwoByTwoArea))
                {
                    physicalAdjacent2 &= ~Connection.Southwest;
                }
            }

            if ((physicalAdjacent2 & Connection.Southeast) == Connection.Southeast)
            {
                TwoByTwoArea[0] = adjacentSections[6];
                TwoByTwoArea[1] = adjacentSections[7];
                TwoByTwoArea[2] = adjacentSections[0];
                TwoByTwoArea[3] = thisSection;

                if (!Check2x2(TwoByTwoArea))
                {
                    physicalAdjacent2 &= ~Connection.Southeast;
                }
            }

            /// <summary>
            /// Checks a 2x2 group of sections to make sure they're valid.
            /// </summary>
            /// <returns>The check2x2.</returns>
            /// <param name="sections">Sections.</param>
            bool Check2x2(Section?[] sections)
            {
                // Check none are null
                for (int i = 0; i < 4; i++)
                {
                    if (sections[i] == null)
                    {
                        return false;
                    }
                }

                // We do 3 checks per each element of the 2x2 in the following pattern:
                // 0, 1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 0

                for (int i = 0; i < 4; i++)
                {
                    if (!((Section)sections[i]).CanConnect(connectionsToCheck[(3 * i) + 0]))
                    {
                        return false;
                    }
                    if (!((Section)sections[i]).CanConnect(connectionsToCheck[(3 * i) + 1]))
                    {
                        return false;
                    }
                    if (!((Section)sections[i]).CanConnect(connectionsToCheck[(3 * i) + 2]))
                    {
                        return false;
                    }
                }

                return true;
            }

            Profiler.EndSample();

            return physicalAdjacent2;
        }
    }
}
