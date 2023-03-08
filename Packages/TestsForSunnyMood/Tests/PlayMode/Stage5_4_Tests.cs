using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

[Description(""), Category("5")]
public class Stage5_4_Tests
{
    private GameObject player;
    private GameObject[] platforms;
    private GameObject grid, ground;
    private Vector2 playerSize;
    private float jumpheight;
    private LayerMask groundLayer;

    [UnityTest, Order(1)]
    public IEnumerator NecessaryComponents()
    {
        if (!PMHelper.CheckTagExistance("Platform"))
        {
            Assert.Fail("Level 3: \"Platform\" tag was not added to project");
        }
        
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

        player = GameObject.Find("Player");
        platforms = GameObject.FindGameObjectsWithTag("Platform");
        if (platforms.Length == 0)
        {
            Assert.Fail("Level 3: Platforms were not added to a scene or their tag misspelled");
        }

        grid = GameObject.Find("Grid");
        ground = GameObject.Find("Ground");
        GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");
        foreach (GameObject b in bounds)
        {
            b.layer = LayerMask.NameToLayer("Test");
        }

        groundLayer = ground.layer;
        ground.layer = LayerMask.NameToLayer("Test");

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            ground.layer == LayerMask.NameToLayer("Test") || (Time.unscaledTime - start) * Time.timeScale > 1);

        foreach (GameObject platform in platforms)
        {
            if (!PMHelper.Child(platform, grid))
            {
                Assert.Fail(
                    "Level 3: Platforms should be children of \"Grid\" object, because tilemap should be a child of grid");
            }

            Tilemap platformTM = PMHelper.Exist<Tilemap>(platform);
            TilemapRenderer platformTMR = PMHelper.Exist<TilemapRenderer>(platform);
            if (!platformTM)
                Assert.Fail("Level 3: There should be a <Tilemap> component on \"Platform\"s' object to use tilemaps");
            if (!platformTMR)
                Assert.Fail(
                    "Level 3: There should be a <TilemapRenderer> component on \"Platform\"s' object to view created tilemaps");
            if (platformTM.size.x <= 0 || platformTM.size.y <= 0)
                Assert.Fail("Level 3: There should be added tiles to \"Platform\"s' tilemap");

            GameObject background = GameObject.Find("Background");
            SpriteRenderer backgroundSR = PMHelper.Exist<SpriteRenderer>(background);

            if (backgroundSR.sortingLayerID != platformTMR.sortingLayerID)
            {
                Assert.Fail("Level 3: There is no need here to create new sorting layers. " +
                            "Set all the <SpriteRenderer>s on the same sorting layer. To order visibility you should change their " +
                            "\"Order in layer\" property");
            }

            if (platformTMR.sortingOrder <= backgroundSR.sortingOrder)
            {
                Assert.Fail("Level 3: \"Platform\"'s order in layer should be greater than \"Background\"'s one");
            }

            BoxCollider2D platformCL = PMHelper.Exist<BoxCollider2D>(platform);
            if (!platformCL || !platformCL.enabled)
            {
                Assert.Fail("Level 3: \"Platform\"s' objects should have an assigned enabled <Collider2D> component");
            }

            if (platformCL.isTrigger)
            {
                Assert.Fail("Level 3: \"Platform\"s' <Collider2D> component should not be triggerable");
            }

            if (!platformCL.sharedMaterial || platformCL.sharedMaterial.friction != 0)
            {
                Assert.Fail(
                    "Level 3: \"Platform\"s' <Collider2D> component should have assigned <2D Physics Material> with " +
                    "friction set to zero, so the player won't be able to hang on the edge of platforms");
            }
            
            if (!PMHelper.RaycastFront2D(platformCL.bounds.center, Vector2.down,
                1 << LayerMask.NameToLayer("Test")).collider)
            {
                Assert.Fail("Level 3: All the platforms should be placed above the \"Ground\"'s object");
            }

            if (!PMHelper.RaycastFront2D(platformCL.bounds.center, Vector2.left,
                1 << LayerMask.NameToLayer("Test")).collider)
            {
                Assert.Fail("Level 3: All the platforms should be placed between bounds; " +
                            "Bound should be higher than all the platforms");
            }

            if (!PMHelper.RaycastFront2D(platformCL.bounds.center, Vector2.right,
                1 << LayerMask.NameToLayer("Test")).collider)
            {
                Assert.Fail("Level 3: All the platforms should be placed between bounds; " +
                            "Bound should be higher than all the platforms");
            }

            playerSize = player.GetComponent<Collider2D>().bounds.size;
            if (playerSize.x * 2 >= platformCL.size.x)
            {
                Assert.Fail(
                    "Level 3: Let \"Platform\"s' <Collider2D> component be at least twice as wider, than player's one");
            }
        }

        ground.layer = groundLayer;
    }

    [UnityTest, Order(2)]
    public IEnumerator CheckCollisions()
    {
        GameObject platformTmp = platforms[0];
        Collider2D tmp = PMHelper.Exist<Collider2D>(platformTmp);

        Vector3 minPlace = tmp.bounds.center;

        foreach (GameObject platform in platforms)
        {
            tmp = PMHelper.Exist<Collider2D>(platform);
            if (minPlace.y < tmp.bounds.center.y)
            {
                GameObject.Destroy(platform);
            }
            else
            {
                platformTmp = platform;
                minPlace = tmp.bounds.center;
            }
        }

        Collider2D groundCL = PMHelper.Exist<Collider2D>(ground);
        Collider2D platformCL = PMHelper.Exist<Collider2D>(platformTmp);
        Collider2D playerCL = PMHelper.Exist<Collider2D>(player);
        Rigidbody2D playerRB = PMHelper.Exist<Rigidbody2D>(player);
        
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCL.IsTouching(groundCL) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if ((Time.unscaledTime - start) * Time.timeScale >= 5)
        {
            Assert.Fail(
                "Level 1: In some time after the scene was loaded \"Player\"'s collider should be touching \"Ground\"'s collider");
        }
        
        if (playerCL.bounds.max.y >= platformCL.bounds.min.y)
        {
            Assert.Fail("Level 3: Platform with the lowest Y-axis should be placed higher than a player");
        }

        platformCL.enabled = false;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !platformCL.enabled || (Time.unscaledTime - start) * Time.timeScale > 1);

        Time.timeScale = 1;


        Vector2 playerPlace = playerCL.bounds.min;

        VInput.KeyPress(KeyCode.Space);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.velocity.y < 0 || (Time.unscaledTime - start) * Time.timeScale > 5);
        if ((Time.unscaledTime - start) * Time.timeScale >= 5)
        {
            Assert.Fail(
                "Level 3: Y-axis velocity of <Rigidbody2D> component should decrease after the jump was provided," +
                " because of the gravity, and after some time it should become negative");
        }

        float jumpPlace = playerCL.bounds.min.y;
        jumpheight = jumpPlace - playerPlace.y;

        if (jumpPlace <= platformCL.bounds.max.y)
        {
            Assert.Fail("Level 3: Player should be able to jump on platform");
        }

        Time.timeScale = 3;

        platformCL.enabled = true;
        yield return new WaitUntil(() =>
            platformCL.enabled || (Time.unscaledTime - start) * Time.timeScale > 1);
        
        player.transform.position =
            new Vector3(platformCL.bounds.center.x, platformCL.bounds.max.y + jumpheight);

        yield return new WaitUntil(() =>
            playerCL.IsTouching(platformCL) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if ((Time.unscaledTime - start) * Time.timeScale >= 5)
        {
            Assert.Fail("Level 3: When the player is falling down to the ground, and there is a platform on it's way," +
                        " player should fall down to the platform.");
        }

        yield return new WaitForSeconds(1);

        if (playerRB.velocity.y != 0)
        {
            Assert.Fail("Level 3: Player should not be able to fall through the platform. " +
                        "Player should be standing still on it");
        }

        VInput.KeyPress(KeyCode.Space);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.velocity.y > 0 || (Time.unscaledTime - start) * Time.timeScale > 2);
        if ((Time.unscaledTime - start) * Time.timeScale >= 2)
        {
            Assert.Fail("Level 3: When the Space-button was pressed (while player is standing on a platform)" +
                        " jump should be provided, so the Rigidbody2D's Y-axis velocity should increase");
        }
    }

    [UnityTest, Order(3)]
    public IEnumerator PlatformsReachableCheck()
    {
        List<PlatformInfo> infos = new List<PlatformInfo>();
        Time.timeScale = 3;

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

        foreach (GameObject platform in GameObject.FindGameObjectsWithTag("Platform"))
        {
            infos.Add(platform.AddComponent<PlatformInfo>());
        }

        ground = GameObject.Find("Ground");
        PlatformInfo grInfo = ground.AddComponent<PlatformInfo>();
        grInfo.reachable = true;
        foreach (PlatformInfo info in infos)
        {
            if (Mathf.Abs(info.leftUp.y - grInfo.leftUp.y) < jumpheight)
            {
                info.reachable = true;
            }

            foreach (PlatformInfo info2 in infos)
            {
                if (info == info2 || info2.reachable || info2.leftUp.y < info.leftUp.y) continue;
                if (Mathf.Abs(info2.leftUp.x - info.rightUp.x) < jumpheight &&
                    Mathf.Abs(info2.leftUp.y - info.rightUp.y) < jumpheight
                    ||
                    Mathf.Abs(info2.rightUp.x - info.leftUp.x) < jumpheight &&
                    Mathf.Abs(info2.rightUp.y - info.leftUp.y) < jumpheight
                )
                {
                    info2.reachable = true;
                    //info2.gameObject.GetComponent<Tilemap>().color=Color.green;
                }
                else
                {
                    info2.reachable = false;
                    //info2.gameObject.GetComponent<Tilemap>().color=Color.red;
                }

                yield return null;
            }
        }

        foreach (PlatformInfo info in infos)
        {
            /*if (info.reachable)
            {
                info.gameObject.GetComponent<Tilemap>().color=Color.green;
            }
            else
            {
                info.gameObject.GetComponent<Tilemap>().color=Color.red;
            }

            Time.timeScale = 1;
            yield return new WaitForSeconds(0.5f);*/

            if (!info.reachable)
            {
                Assert.Fail("Level 3: All platforms should be conveniently reachable from lower platforms");
            }
        }
    }

    [UnityTest, Order(4)]
    public IEnumerator CorrectGemsNExit()
    {
        yield return null;
        foreach (GameObject gem in GameObject.FindGameObjectsWithTag("Gem"))
        {
            Collider2D gemColl = gem.GetComponent<Collider2D>();
            Collider2D[] colls = Physics2D.OverlapCircleAll(gemColl.bounds.center, jumpheight);
            /*Debug.DrawLine(gemColl.bounds.center,gemColl.bounds.center+Vector3.down*jumpheight,Color.blue,10);
            Debug.DrawLine(gemColl.bounds.center,gemColl.bounds.center+Vector3.up*jumpheight,Color.blue,10);
            Debug.DrawLine(gemColl.bounds.center,gemColl.bounds.center+Vector3.left*jumpheight,Color.blue,10);
            Debug.DrawLine(gemColl.bounds.center,gemColl.bounds.center+Vector3.right*jumpheight,Color.blue,10);
            yield return new WaitForSeconds(10);*/
            Collider2D platform = Array.Find(colls, plat =>
                plat.gameObject.tag == "Platform" ||
                plat.gameObject.name.Equals("Ground"));
            if (!platform)
            {
                Assert.Fail("Level 3: It should not be hard or impossible to collect gems");
            }
        }

        Collider2D endColl = GameObject.Find("LevelEnd").GetComponent<Collider2D>();
        Collider2D[] colls2 = Physics2D.OverlapCircleAll(endColl.bounds.center, jumpheight);
        Collider2D platform2 = Array.Find(colls2, plat =>
            plat.gameObject.CompareTag("Platform") ||
            plat.gameObject.name.Equals("Ground"));
        if (!platform2)
        {
            Assert.Fail("Level 3: It should not be hard or impossible to reach \"LevelEnd\" object");
        }
    }
}