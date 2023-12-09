using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSFramework.Animation;

namespace FPSFramework.Examples
{
    //This is a tutorial.
    //Please read the code and comments for a better understanding of the system, and remember that this is just a starting point.
    //The reason why I didn't use Procedural Animator Trigger instead is that you have much more control over your animation with this.
    //PA stands for Procedural Animator.
    public class ProceduralAnimatorControllerExample : MonoBehaviour
    {
        //You can cash this as well, but let's just keep it simple.
        public ProceduralAnimator animator;

        //You can refer to all the clips directly, but I don't like it that way.
        public string ADSName = "Aim Down Sight";
        public string recoilName = "Recoil";
        public string sprintName = "Sprint";
        public string breathName = "Breathing";

        public float fireRate = 10;
        [Range(0, 1)] public float aimRecoil = 0.5f;
        [Range(0, 1)] public float aimBreath = 0.5f;
        [Range(0, 120)] public float aimFOV;

        //You can get the clips in the update, but saving them helps a lot with the performance.

        private ProceduralAnimation ADSClip;
        private ProceduralAnimation recoilClip;
        private ProceduralAnimation sprintClip;
        private ProceduralAnimation breathClip;
        private Camera cam;

        private void Start()
        {
            //This is how you can get a clip directly from the animator if you want to reference it directly.
            ADSClip = animator?.GetClip(ADSName);
            recoilClip = animator?.GetClip(recoilName);
            sprintClip = animator.GetClip(sprintName);
            breathClip = animator.GetClip(breathName);

            //I'm cashing in on cam value to save a lot of performance, and the same for animation clips.
            cam = Camera.main;
        }

        private void Update()
        {
            //I'm using SetTrigger(bool value) instead of directly setting isTriggered because this saves a bit of time and makes your code more readable.
            ADSClip?.SetTrigger(Input.GetKey(KeyCode.Mouse1));
            recoilClip?.SetTrigger(Input.GetKey(KeyCode.Mouse0));
            sprintClip?.SetTrigger(Input.GetKey(KeyCode.LeftShift));

            

            //Setting weight value to chnage the effect amount and because the progress value ranges from 0 to 1, it is compatible with lerp. 

            float currentRecoil = Mathf.Lerp(1, aimRecoil, ADSClip.progress);
            recoilClip.weight = currentRecoil;

            float currentBreath = Mathf.Lerp(1, aimBreath, ADSClip.progress);
            breathClip.weight = currentBreath;

            float currentFOV = Mathf.Lerp(60, aimFOV, ADSClip.progress);
            cam.fieldOfView = currentFOV;
        }
    }
}