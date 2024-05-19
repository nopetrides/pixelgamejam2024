using UnityEngine;
using UnityEngine.Assertions;

namespace Febucci.UI.Examples
{
    /// <summary>
    /// Extra example class for the TextAnimator plugin, used to add sounds to the TextAnimatorPlayer.
    /// </summary>
    [AddComponentMenu("Febucci/TextAnimator/SoundWriter")]
    [RequireComponent(typeof(Core.TypewriterCore))]
    public class TAnimSoundWriter : MonoBehaviour
    {
        [Header("Management")]
        [Tooltip("How much time has to pass before playing the next sound"), SerializeField, Attributes.MinValue(0)]
        float minSoundDelay = .07f;

        [Tooltip("True if you want the new sound to cut the previous one\nFalse if each sound will continue until its end"), SerializeField]
        bool interruptPreviousSound = true;

        [Header("Audio Clips")]
        [Tooltip("True if sounds will be picked random from the array\nFalse if they'll be chosen in order"), SerializeField]
        bool randomSequence = false;
        [SerializeField] AudioClip[] sounds = new AudioClip[0];

        float latestTimePlayed = -1;
        int clipIndex;

        private void Start()
        {
            Assert.IsNotNull(AudioManager.Instance, "TAnimSoundWriter: Typewriter Audio Source reference is null");
            Assert.IsNotNull(sounds, "TAnimSoundWriter: Sounds clips array is null in the");
            Assert.IsTrue(sounds.Length > 0, "TAnimSoundWriter: Sounds clips array is empty");
            Assert.IsNotNull(GetComponent<Core.TypewriterCore>(), "TAnimSoundWriter: Component TAnimPlayerBase is not present");


            

            GetComponent<Core.TypewriterCore>()?.onCharacterVisible.AddListener(OnCharacter);

            clipIndex = randomSequence ? Random.Range(0, sounds.Length) : 0;
        }

        void OnCharacter(char character)
        {
            if (Time.time - latestTimePlayed <= minSoundDelay)
                return; //Early return if not enough time passed yet

            var clip = sounds[clipIndex];

            //Plays audio
            if (interruptPreviousSound)
                AudioManager.Instance.PlaySound(clip, 1f);
            else
                AudioManager.Instance.PlaySound(clip, 1f);

            //Chooses next clip to play
            if (randomSequence)
            {
                clipIndex = Random.Range(0, sounds.Length);
            }
            else
            {
                clipIndex++;
                if (clipIndex >= sounds.Length)
                    clipIndex = 0;
            }

            latestTimePlayed = Time.time;

        }
    }
}