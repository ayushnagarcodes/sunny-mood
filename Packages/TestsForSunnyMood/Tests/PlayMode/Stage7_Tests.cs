using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("I'd rather chase the Sun than wait for it."), Category("7")]
public class Stage7_Tests
{
    private GameObject player, gem, ground;
    private AudioSource sTheme, sJump, sGem;

    [UnityTest, Order(0)]
    public IEnumerator InGameSoundCheck()
    {
        Time.timeScale = 5;
        SceneManager.LoadScene("Main Menu");
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Main Menu" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            Assert.Fail("\"Main Menu\" scene can't be loaded");
        }

        yield return new WaitUntil(() =>
            PMHelper.AudioSourcePlaying("Theme") || (Time.unscaledTime - start) * Time.timeScale > 1);

        sTheme = PMHelper.AudioSourcePlaying("Theme");
        if (!sTheme)
            Assert.Fail("There is no \"Theme\" AudioSource on \"Main Menu\" scene");
        if (!sTheme.isPlaying)
            Assert.Fail("\"Theme\" AudioSource should be playing on \"Main Menu\"");
        if (!sTheme.loop)
            Assert.Fail("\"Theme\" AudioSource should be looped");

        SceneManager.LoadScene("Level 1");
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Level 1" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Level 1")
        {
            Assert.Fail("\"Level 1\" scene can't be loaded");
        }

        yield return new WaitUntil(() =>
            PMHelper.AudioSourcePlaying("Theme") || (Time.unscaledTime - start) * Time.timeScale > 1);
        
        sTheme = PMHelper.AudioSourcePlaying("Theme");
        if (!sTheme)
            Assert.Fail("There is no \"Theme\" AudioSource on levels' scenes");
        if (!sTheme.isPlaying)
            Assert.Fail("\"Theme\" AudioSource should be playing on levels' scenes");
        if (!sTheme.loop)
            Assert.Fail("\"Theme\" AudioSource should be looped");
        
        player = GameObject.Find("Player");
        ground = GameObject.Find("Ground");
        gem = GameObject.FindWithTag("Gem");

        Collider2D playerCL = PMHelper.Exist<Collider2D>(player);
        Collider2D groundCL = PMHelper.Exist<Collider2D>(ground);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCL.IsTouching(groundCL) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if ((Time.unscaledTime - start) * Time.timeScale >= 5)
        {
            Assert.Fail(
                "Level 1: In some time after the scene was loaded \"Player\"'s collider should be touching \"Ground\"'s collider");
        }

        VInput.KeyPress(KeyCode.Space);
        
        yield return new WaitUntil(() =>
            PMHelper.AudioSourcePlaying("Jump") || (Time.unscaledTime - start) * Time.timeScale > 1);
        
        sJump = PMHelper.AudioSourcePlaying("Jump");
        if (!sJump)
            Assert.Fail("There is no \"Jump\" AudioSource when jump was performed");
        if (!sJump.isPlaying)
            Assert.Fail("\"Jump\" AudioSource should be played when player's jump is performed");
        if (sJump.loop)
            Assert.Fail("\"Jump\" AudioSource should not be looped");
        
        
        gem.transform.position = playerCL.bounds.center;
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !gem || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (gem)
        {
            Assert.Fail("Level 1: \"Gem\"s should be destroyed instantly when the player collides with them");
        }

        yield return new WaitUntil(() =>
            PMHelper.AudioSourcePlaying("PickGem") || (Time.unscaledTime - start) * Time.timeScale > 1);
        
        sGem = PMHelper.AudioSourcePlaying("PickGem");
        if (!sGem)
            Assert.Fail("There is no \"PickGem\" AudioSource when gem was collected");
        if (!sGem.isPlaying)
            Assert.Fail("\"PickGem\" sound should be played when player has collected gem");
        if (sGem.loop)
            Assert.Fail("\"PickGem\" sound should not be looped");
    }
}