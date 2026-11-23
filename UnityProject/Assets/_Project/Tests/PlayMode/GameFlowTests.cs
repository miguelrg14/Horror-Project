using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Horror.Tests
{
    /// <summary>Loads the real scenes and checks the menus, the pause flow, the audio channels and the credits.</summary>
    public class GameFlowTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            Time.timeScale = 1f;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Time.timeScale = 1f;
            yield return null;
        }

        private static IEnumerator LoadScene(string name, float settleSeconds = 1f)
        {
            yield return SceneManager.LoadSceneAsync(name);
            yield return new WaitForSecondsRealtime(settleSeconds);
            Assert.AreEqual(name, SceneManager.GetActiveScene().name);
        }

        [UnityTest]
        public IEnumerator MainMenu_HasALoopingThemeOnTheMusicChannel()
        {
            yield return LoadScene("Menu");

            GameObject menuMusic = GameObject.Find("MenuMusic");
            Assert.IsNotNull(menuMusic, "The menu scene has its theme.");
            var source = menuMusic.GetComponent<AudioSource>();
            Assert.IsNotNull(source.clip);
            Assert.IsTrue(source.loop);
            Assert.IsTrue(source.playOnAwake);
            Assert.IsNotNull(source.outputAudioMixerGroup, "The theme goes through the mixer so the music slider controls it.");
            StringAssert.Contains("Music", source.outputAudioMixerGroup.name);
        }

        [UnityTest]
        public IEnumerator Level1_PauseAndResume()
        {
            yield return LoadScene("Level1", 2f);

            Component pauseMenu = GameTypes.Find("PauseMenu");
            Assert.IsNotNull(pauseMenu);
            System.Type pauseType = GameTypes.Get("PauseMenu");

            GameTypes.Call(pauseMenu, "Pause");
            yield return null;
            Assert.AreEqual(0f, Time.timeScale);
            Assert.IsTrue((bool)GameTypes.Get(pauseType, "IsPaused"));

            GameTypes.Call(pauseMenu, "HandleEscape"); // Escape while paused resumes
            yield return null;
            Assert.AreEqual(1f, Time.timeScale);
            Assert.IsFalse((bool)GameTypes.Get(pauseType, "IsPaused"));
        }

        [UnityTest]
        public IEnumerator Level1_PlayerSoundsPlayOnceAndCleanUp()
        {
            yield return LoadScene("Level1", 2f);

            Component sfx = GameTypes.Find("PlayerSFXManager");
            Assert.IsNotNull(sfx);

            var pick = (AudioSource)GameTypes.Call(sfx, "PlaySFX", "pickKey");
            Assert.IsNotNull(pick);
            Assert.IsFalse(pick.loop, "One-shot sounds do not inherit the template's loop flag.");
            Assert.IsNotNull(pick.clip);

            // Let it start, then end it (CI machines have no sound card, so clips never finish on their own).
            yield return null;
            GameObject voice = pick.gameObject;
            pick.Stop();
            yield return null;
            yield return null;
            Assert.IsTrue(voice == null, "The sound object removes itself once its clip has stopped.");

            var loop = (AudioSource)GameTypes.Call(sfx, "PlaySFX", "heartBeat2");
            GameTypes.Call(sfx, "Loop");
            yield return null;
            Assert.IsTrue(loop != null && loop.loop, "Loop() keeps the last sound playing.");
            GameTypes.Call(sfx, "Stop");
            yield return null;
            Assert.IsTrue(loop == null, "Stop() removes the looping sound.");

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("no sound called 'doesNotExist'"));
            Assert.IsNull(GameTypes.Call(sfx, "PlaySFX", "doesNotExist"));
        }

        [UnityTest]
        public IEnumerator Credits_ReturnToTheMainMenu()
        {
            yield return LoadScene("Credits", 0.1f);

            Time.timeScale = 10f; // the credits last 10 game seconds
            float timeout = 10f;
            while (SceneManager.GetActiveScene().name != "Menu" && timeout > 0f)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            Time.timeScale = 1f;

            Assert.AreEqual("Menu", SceneManager.GetActiveScene().name);
        }
    }
}
