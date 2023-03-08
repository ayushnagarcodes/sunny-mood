using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("The low sun stares through dust of gold."), Category("5")]
public class Stage5_Tests
{
    private GameObject[] enemies;
    private GameObject player;

    private GameObject rat;
    private Animator ratAnim;
    private AnimationClip[] ratAclips;

    [UnityTest, Order(0)]
    public IEnumerator CheckScene()
    {
        Time.timeScale = 0;
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

        if (!PMHelper.CheckTagExistance("Enemy"))
        {
            Assert.Fail("Level 3: \"Enemy\" tag was not added to project");
        }

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Assert.Fail("Level 3: Enemies were not added to a scene or their tag misspelled");
        }
    
        GameObject background = GameObject.Find("Background");
        
        SpriteRenderer backgroundSR = PMHelper.Exist<SpriteRenderer>(background);

        foreach (GameObject rat in enemies)
        {
            SpriteRenderer ratSR = PMHelper.Exist<SpriteRenderer>(rat);
            if (!ratSR || !ratSR.enabled)
                Assert.Fail("Level 3: There is no <SpriteRenderer> component on \"Enemy\"'s objects or it is disabled");
            if (!ratSR.sprite)
                Assert.Fail("Level 3: There should be sprite, attached to \"Enemy\"'s objects' <SpriteRenderer>");

            if (backgroundSR.sortingLayerID != ratSR.sortingLayerID)
            {
                Assert.Fail("Level 3: There is no need here to create new sorting layers. " +
                            "Set all the <SpriteRenderer>s on the same sorting layer. To order visibility you should change their" +
                            "\"Order in layer\" property");
            }

            if (ratSR.sortingOrder <= backgroundSR.sortingOrder)
            {
                Assert.Fail("Level 3: \"Enemy\"'s order in layer should be greater than \"Background\"'s one");
            }

            Collider2D ratCL = PMHelper.Exist<Collider2D>(rat);
            if (!ratCL || !ratCL.enabled)
            {
                Assert.Fail("Level 3: \"Enemy\" objects should have assigned enabled <Collider2D> component");
            }

            if (!ratCL.isTrigger)
            {
                Assert.Fail("Level 3: \"Enemy\" objects' <Collider2D> component should be triggerable");
            }

            ratAnim = PMHelper.Exist<Animator>(rat);
            if (!ratAnim)
            {
                Assert.Fail("Level 3: There is no attached <Animator> component to enemies");
            }

            if (!ratAnim.runtimeAnimatorController)
            {
                Assert.Fail("Level 3: There should be created controller, attached to enemies' <Animator> component!");
            }
            
            ratAclips = ratAnim.runtimeAnimatorController.animationClips;
        }
        
        if (ratAclips.Length != 1)
        {
            Assert.Fail("Level 3: There should be added only 1 clip to enemies' animator: \"EnemyWalk\"");
        }

        AnimationClip walk = Array.Find(ratAclips, clip => clip.name.Equals("EnemyWalk"));

        if (walk == null) Assert.Fail("Level 3: There should be a clip in enemies' animator, called \"EnemyWalk\"");
        if (walk.legacy)
            Assert.Fail("Level 3: \"EnemyWalk\" clip should be animated by animator, not by the <Legacy Animation>" +
                        " component, so it's legacy property should be unchecked");
        if (walk.empty) Assert.Fail("Level 3: \"EnemyWalk\" clip in enemies' animator should have animation keys");
        if (!walk.isLooping) Assert.Fail("Level 3: \"EnemyWalk\" clip in enemies' animator should be looped");

        
        if (ratAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "EnemyWalk")
        {
            Assert.Fail("Level 3: \"EnemyWalk\" clip should be played by default");
        }
    }

    [UnityTest, Order(3)]
    public IEnumerator CorrectPlacementAndMovement()
    {
        PMHelper.TurnCollisions(true);
        Time.timeScale = 5;
        foreach (GameObject pl in GameObject.FindGameObjectsWithTag("Platform"))
        {
            pl.layer = LayerMask.NameToLayer("Test");
        }

        rat = GameObject.FindWithTag("Enemy");
        Collider2D ratCL = rat.GetComponent<Collider2D>();
        if (!PMHelper.RaycastFront2D(ratCL.bounds.center, Vector2.down,
            1 << LayerMask.NameToLayer("Test")).collider)
        {
            Assert.Fail("Level 3: Enemies should be spawned on platforms, right above them");
        }

        SpriteRenderer sr = rat.GetComponent<SpriteRenderer>();
        bool rotated = sr.flipX;

        Vector3 firstPos = rat.transform.position;
        
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            rat.transform.position != firstPos || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (firstPos == rat.transform.position)
        {
            Assert.Fail("Level 3: Enemies are not moving");
        }
        
        bool movingLeft = firstPos.x < rat.transform.position.x;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            sr.flipX != rotated || (Time.unscaledTime - start) * Time.timeScale > 6);
        if ((Time.unscaledTime - start) * Time.timeScale >= 6)
        {
            Assert.Fail(
                "Level 3: Enemies should change their movement direction and flip their sprite in less than 5 seconds");
        }

        firstPos = rat.transform.position;
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            rat.transform.position != firstPos || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (firstPos == rat.transform.position)
        {
            Assert.Fail("Level 3: Enemies are not moving");
        }
        if (movingLeft == firstPos.x < rat.transform.position.x)
        {
            Assert.Fail(
                "Level 3: Enemies should change their movement direction and flip their sprite in less than 5 seconds");
        }
    }

    [UnityTest, Order(4)]
    public IEnumerator CollisionCheck()
    {
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

        player = GameObject.Find("Player");
        rat = GameObject.FindWithTag("Enemy");

        Scene cur = SceneManager.GetActiveScene();
        String name = cur.name;
        
        player.transform.position = rat.transform.position;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene() != cur || (Time.unscaledTime - start) * Time.timeScale > 1);
        if ((Time.unscaledTime - start) * Time.timeScale >= 1)
        {
            Assert.Fail("Level 3: When \"Player\" collides with an \"Enemy\" object - scene should reload");
        }

        if (SceneManager.GetActiveScene().name != name)
        {
            Assert.Fail("Level 3: When \"Player\" collides with an \"Enemy\" object - scene should reload");
        }
    }
}