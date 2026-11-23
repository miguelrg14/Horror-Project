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
        public IEnumerator MainMenu_StartsTheLoopingMenuMusic()
        {
            yield return LoadScene("Menu");

            Component music = GameTypes.Find("PlayerMusicManager");
            Assert.IsNotNull(music, "The menu has a music channel.");
            var current = (AudioSource)GameTypes.Get(music, "Current");
            Assert.IsNotNull(current, "Menu music was started.");
            Assert.IsNotNull(current.clip);
            Assert.IsTrue(current.loop, "The menu theme loops instead of being cut after one play.");
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
            // CI machines have no sound card; without an output device nothing ever "plays".
            Assume.That(pick.isPlaying, "No audio output device available.");

            float timeout = pick.clip.length + 2f;
            GameObject voice = pick.gameObject;
            while (voice != null && timeout > 0f)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            Assert.IsTrue(voice == null, "The sound object removes itself when the clip ends.");

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
