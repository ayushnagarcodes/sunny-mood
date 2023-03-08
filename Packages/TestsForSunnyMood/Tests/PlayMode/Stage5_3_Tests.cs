using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.SceneManagement;

[Description(""), Category("5")]
public class Stage5_3_Tests
{
    private GameObject player, gem, levelEnd;
    private SpriteRenderer sr, gemSR;
    private Animator anim, gemAnim;
    private AnimationClip[] aclips, gemAclips;
    private AnimationClip idle, jump, walk, gemClip;
    private Collider2D playerCl, groundCl, gemCL, levelEndCl;


    [UnityTest, Order(1)]
    public IEnumerator NecessaryComponents()
    {
        PMHelper.TurnCollisions(true);
        
        if (!Application.CanStreamedLevelBeLoaded("Level 3"))
        {
            Assert.Fail("\"Level 3\" scene is misspelled or was not added to build settings");
        }
        
        SceneManager.LoadScene("Level 3");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Level 3" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Level 3")
        {
            Assert.Fail("\"Level 3\" scene can't be loaded");
        }
        
        if (PMHelper.CheckTagExistance("Platform"))
        {
            foreach (GameObject platform in GameObject.FindGameObjectsWithTag("Platform"))
            {
                GameObject.Destroy(platform);
            }
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                GameObject.FindGameObjectsWithTag("Platform").Length!=0 || (Time.unscaledTime - start) * Time.timeScale > 1);
        }
        if (PMHelper.CheckTagExistance("Enemy"))
        {
            foreach (GameObject platform in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                GameObject.Destroy(platform);
            }
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                GameObject.FindGameObjectsWithTag("Enemy").Length!=0 || (Time.unscaledTime - start) * Time.timeScale > 1);
        }

        levelEnd = GameObject.Find("LevelEnd");
        levelEndCl = PMHelper.Exist<Collider2D>(levelEnd);
        levelEndCl.enabled = false;

        player = GameObject.Find("Player");
        
        
        playerCl = PMHelper.Exist<Collider2D>(player);
        sr = PMHelper.Exist<SpriteRenderer>(player);
        anim = PMHelper.Exist<Animator>(player);

        if (!anim || !anim.enabled)
        {
            Assert.Fail(
                " Level 3: Player should have assigned enabled <Animator> component in order to perform animations");
        }

        if (!anim.runtimeAnimatorController)
        {
            Assert.Fail(" Level 3: There should be created controller, attached to <Animator> component");
        }

        aclips = anim.runtimeAnimatorController.animationClips;

        if (aclips.Length != 3)
        {
            Assert.Fail(" Level 3: There should be added 3 clips to \"Player\"'s animator: Idle, Jump, Walk");
        }

        foreach (var clipName in new[] {"Idle", "Jump", "Walk"})
        {
            AnimationClip clip = Array.Find(aclips, clip => clip.name.Equals(clipName));

            if (!clip) Assert.Fail(" Level 3: There should be a clip in \"Player\"'s animator, called \""+clipName+"\"");
            if (clip.legacy)
                Assert.Fail(" Level 3: \""+clipName+"\" clip should be animated by animator, not by the <Legacy Animation>" +
                            " component, so it's legacy property should be unchecked");
            if (clip.empty) Assert.Fail(" Level 3: \""+clipName+"\" clip in Player's animator should have animation keys");
            if (!clip.isLooping) Assert.Fail(" Level 3: \""+clipName+"\" clip in Player's animator should be looped");
        }

        GameObject ground = GameObject.Find("Ground");
        groundCl = PMHelper.Exist<Collider2D>(ground);
    }

    [UnityTest, Order(3)]
    public IEnumerator CheckTransitions()
    {
        Time.timeScale = 10;

        if (sr.flipX)
        {
            Assert.Fail(" Level 3: By default \"Player\"'s sprite should not be flipped");
        }
        
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(groundCl) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if ((Time.unscaledTime - start) * Time.timeScale >= 5)
        {
            Assert.Fail(
                "Level 3: In some time after the scene was loaded \"Player\"'s collider should be touching \"Ground\"'s collider");
        }
        
        yield return new WaitForSeconds(1);
        
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" ||
            (Time.unscaledTime - start) * Time.timeScale > 2);
        if ((Time.unscaledTime - start) * Time.timeScale >= 2)
        {
            Assert.Fail(" Level 3: While character is not moving - \"Idle\" clip should be played");
        }

        VInput.KeyDown(KeyCode.D);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Walk" ||
            (Time.unscaledTime - start) * Time.timeScale > 2);
        if ((Time.unscaledTime - start) * Time.timeScale >= 2)
        {
            Assert.Fail(
                " Level 3: While character is moving on the ground to the right - \"Walk\" clip should be played");
        }

        VInput.KeyPress(KeyCode.Space);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump" ||
            (Time.unscaledTime - start) * Time.timeScale > 2);
        if ((Time.unscaledTime - start) * Time.timeScale >= 2)
        {
            Assert.Fail(" Level 3: While character is in air - \"Jump\" clip should be played");
        }
        
        VInput.KeyUp(KeyCode.D);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(groundCl)  && !playerCl.IsTouching(groundCl)
            || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (!playerCl.IsTouching(groundCl))
        {
            Assert.Fail("Level 3: After the jump is provided, player should fall down to the ground. " +
                        "Jump duration should be less than 2 seconds");
        }

        VInput.KeyDown(KeyCode.A);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Walk" ||
            (Time.unscaledTime - start) * Time.timeScale > 2);
        if ((Time.unscaledTime - start) * Time.timeScale >= 2)
        {
            Assert.Fail(
                " Level 3: While character is moving on the ground to the left - \"Walk\" clip should be played");
        }

        if (!sr.flipX)
        {
            Assert.Fail(
                " Level 3: If last character's movement was performed to the left - it's sprite should be flipped");
        }

        VInput.KeyUp(KeyCode.A);

        VInput.KeyPress(KeyCode.Space);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump" ||
            (Time.unscaledTime - start) * Time.timeScale > 2);
        if ((Time.unscaledTime - start) * Time.timeScale >= 2)
        {
            Assert.Fail(" Level 3: While character is in air - \"Jump\" clip should be played");
        }

        if (!sr.flipX)
        {
            Assert.Fail(
                " Level 3: If last character's movement was performed to the left - it's sprite should be flipped");
        }

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(groundCl) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if ((Time.unscaledTime - start) * Time.timeScale >= 5)
        {
            Assert.Fail(
                " Level 3: In some time after the scene was loaded \"Player\"'s collider should be \"touching\" \"Ground\"'s collider" +
                ". But after 5 seconds of game-time, that didn't happen");
        }

        if (!sr.flipX)
        {
            Assert.Fail(
                " Level 3: If last character's movement was performed to the left - it's sprite should be flipped");
        }
    }

    [UnityTest, Order(4)]
    public IEnumerator NecessaryGemComponents()
    {
        PMHelper.TurnCollisions(false);
        
        if (!Application.CanStreamedLevelBeLoaded("Level 3"))
        {
            Assert.Fail("\"Level 3\" scene is misspelled or was not added to build settings");
        }
        
        SceneManager.LoadScene("Level 3");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Level 3" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Level 3")
        {
            Assert.Fail("\"Level 3\" scene can't be loaded");
        }
        
        if (!PMHelper.CheckTagExistance("Gem"))
        {
            Assert.Fail(" Level 3: \"Gem\" tag was not added to project");
        }
        gem = GameObject.FindWithTag("Gem");
        
        if (!gem)
        {
            Assert.Fail(" Level 3: Gems were not added to a scene or their tag is misspelled");
        }

        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
        
        if (gems.Length != 3)
        {
            Assert.Fail(" Level 3: There should be exactly three gems in scene");
        }

        gemSR = PMHelper.Exist<SpriteRenderer>(gem);
        if (!gemSR || !gemSR.enabled)
            Assert.Fail(" Level 3: There is no <SpriteRenderer> component on \"Gem\" or it is disabled");
        if (!gemSR.sprite)
            Assert.Fail(" Level 3: There should be sprite, attached to \"Gem\" objects' <SpriteRenderer>");

        GameObject background = GameObject.Find("Background");
        SpriteRenderer backgroundSR = PMHelper.Exist<SpriteRenderer>(background);

        if (backgroundSR.sortingLayerID != gemSR.sortingLayerID)
        {
            Assert.Fail(" Level 3: There is no need here to create new sorting layers. " +
                        "Set all the <SpriteRenderer>s on the same sorting layer. To order visibility you should change their" +
                        "\"Order in layer\" property");
        }

        if (gemSR.sortingOrder <= backgroundSR.sortingOrder)
        {
            Assert.Fail(" Level 3: \"Gem\"'s order in layer should be greater than \"Background\"'s one");
        }

        gemCL = PMHelper.Exist<Collider2D>(gem);
        if (!gemCL || !gemCL.enabled)
        {
            Assert.Fail(" Level 3: \"Gem\" objects should have assigned enabled <Collider2D> component");
        }

        if (!gemCL.isTrigger)
        {
            Assert.Fail(" Level 3: \"Gem\" objects's <Collider2D> component should be triggerable");
        }

        gemAnim = PMHelper.Exist<Animator>(gem);
        if (!gemAnim)
        {
            Assert.Fail(" Level 3: There is no attached <Animator> component to gems");
        }

        gemAclips = gemAnim.runtimeAnimatorController.animationClips;
    
        if (gemAclips.Length != 1)
        {
            Assert.Fail(" Level 3: There should be added only 1 clip to Gem's animator, called \"Gem\"");
        }

        gemClip = Array.Find(gemAclips, clip => clip.name.Equals("Gem"));

        if (gemClip == null) Assert.Fail(" Level 3: There should be a clip in Gem's animator, called \"Gem\"");
        if (gemClip.legacy)
            Assert.Fail(" Level 3: \"Gem\" clip should be animated by animator, not by the <Legacy Animation>" +
                        " component, so it's legacy property should be unchecked");
        if (gemClip.empty) Assert.Fail(" Level 3: \"Gem\" clip in Gem's animator should have animation keys");
        if (!gemClip.isLooping) Assert.Fail(" Level 3: \"Gem\" clip in Gem's animator should be looped");
    }

    [UnityTest, Order(6)]
    public IEnumerator CheckTransitionsIdle()
    {
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            gemAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Gem" ||
            (Time.unscaledTime - start) * Time.timeScale > 2);
        if ((Time.unscaledTime - start) * Time.timeScale >= 2)
        {
            Assert.Fail(" Level 3: \"Gem\" clip should be played by default");
        }
    }

    [UnityTest, Order(7)]
    public IEnumerator CheckDestroying()
    {
        player = GameObject.Find("Player");
        gem.transform.position = player.transform.position;
        PMHelper.TurnCollisions(true);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !gem || (Time.unscaledTime - start) * Time.timeScale > 1);
        if ((Time.unscaledTime - start) * Time.timeScale >= 1)
        {
            Assert.Fail(" Level 3: Gems should be destroyed when colliding with Player");
        }
    }
}