using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

[Description(""), Category("5")]
public class Stage5_1_Tests
{
    private GameObject background,
        player,
        grid,
        ground,
        cameraObj,
        levelend;

    private SpriteRenderer playerSR, backgroundSR, levelEndSR;
    private TilemapRenderer groundTMR;

    [UnityTest, Order(1)]
    public IEnumerator Check()
    {
        if (!PMHelper.CheckLayerExistance("Test"))
        {
            Assert.Fail("Please, do not remove \"Test\" layer, it's existence necessary for tests");
        }

        if (!Application.CanStreamedLevelBeLoaded("Level 3"))
        {
            Assert.Fail("\"Level 3\" scene is misspelled or was not added to build settings");
        }

        PMHelper.TurnCollisions(false);
        SceneManager.LoadScene("Level 3");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Level 3" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Level 3")
        {
            Assert.Fail("\"Level 3\" scene can't be loaded");
        }

        //Objects exist
        background = GameObject.Find("Background");
        if (!background)
            Assert.Fail("Level 3: There should be a background, named \"Background\" on scene");

        player = GameObject.Find("Player");
        if (!player)
            Assert.Fail("Level 3: There should be a player, named \"Player\" on scene");

        levelend = GameObject.Find("LevelEnd");
        if (!levelend)
            Assert.Fail("Level 3: There should be a level end, named \"LevelEnd\" on scene");

        //Objects components correct
        backgroundSR = PMHelper.Exist<SpriteRenderer>(background);
        if (!backgroundSR || !backgroundSR.enabled)
            Assert.Fail("Level 3: There is no <SpriteRenderer> component on \"Background\" object or it is disabled");
        if (!backgroundSR.sprite)
            Assert.Fail("Level 3: There should be sprite, attached to \"Background\"'s <SpriteRenderer>");

        playerSR = PMHelper.Exist<SpriteRenderer>(player);
        if (!playerSR || !playerSR.enabled)
            Assert.Fail("Level 3: There is no <SpriteRenderer> component on \"Player\" object or it is disabled");
        if (!playerSR.sprite)
            Assert.Fail("Level 3: There should be sprite, attached to \"Player\"'s <SpriteRenderer>");

        Collider2D playerCl = PMHelper.Exist<Collider2D>(player);
        if (!playerCl || !playerCl.enabled)
        {
            Assert.Fail("Level 3: Player should have assigned enabled <Collider2D> component");
        }

        if (playerCl.isTrigger)
        {
            Assert.Fail("Level 3: Player's <Collider2D> component should not be triggerable");
        }

        Rigidbody2D playerRb = PMHelper.Exist<Rigidbody2D>(player);
        if (!playerRb)
        {
            Assert.Fail("Level 3: Player should have assigned <Rigidbody2D> component");
        }

        if (playerRb.bodyType != RigidbodyType2D.Dynamic)
        {
            Assert.Fail("Level 3: Player's <Rigidbody2D> component should be Dynamic");
        }

        if (!playerRb.simulated)
        {
            Assert.Fail("Level 3: Player's <Rigidbody2D> component should be simulated");
        }

        if (playerRb.gravityScale <= 0)
        {
            Assert.Fail("Level 3: Player's <Rigidbody2D> component should be affected by gravity, " +
                        "so it's Gravity Scale parameter should not be less or equal to 0");
        }

        if (playerRb.interpolation != RigidbodyInterpolation2D.None)
        {
            Assert.Fail("Level 3: Do not change interpolation of Player's <Rigidbody2D> component. Set it as None");
        }

        if (playerRb.constraints != RigidbodyConstraints2D.FreezeRotation)
        {
            Assert.Fail(
                "Level 3: Player's <Rigidbody2D> component's constraints should be freezed by rotation and unfreezed by position");
        }

        levelEndSR = PMHelper.Exist<SpriteRenderer>(levelend);
        if (!levelEndSR || !levelEndSR.enabled)
            Assert.Fail("Level 3: There is no <SpriteRenderer> component on \"LevelEnd\" object or it is disabled");
        if (!levelEndSR.sprite)
            Assert.Fail("Level 3: There should be sprite, attached to \"LevelEnd\"'s <SpriteRenderer>");

        Collider2D levelEndCl = PMHelper.Exist<Collider2D>(levelend);
        if (!levelEndCl || !levelEndCl.enabled)
        {
            Assert.Fail("Level 3: \"LevelEnd\" object should have an assigned enabled <Collider2D> component");
        }

        if (!levelEndCl.isTrigger)
        {
            Assert.Fail("Level 3: \"LevelEnd\"'s <Collider2D> component should be triggerable");
        }
        // Grid check
        grid = GameObject.Find("Grid");
        if (!grid)
        {
            Assert.Fail("Level 3: There should be a tilemap grid, named \"Grid\" on scene");
        }

        ground = GameObject.Find("Ground");
        if (!ground)
        {
            Assert.Fail("Level 3: There should be a ground tilemap, named \"Ground\" on scene");
        }
    
        Grid gridGr = PMHelper.Exist<Grid>(grid);
        if (!gridGr)
            Assert.Fail("Level 3: There should be a <Grid> component on \"Grid\"'s object to use tilemaps");
        if (gridGr.cellLayout != GridLayout.CellLayout.Rectangle)
            Assert.Fail("Level 3: \"Grid\"'s <Grid> component should have Rectangle layout");
        if (gridGr.cellSwizzle != GridLayout.CellSwizzle.XYZ)
            Assert.Fail("Level 3: \"Grid\"'s <Grid> component should have XYZ swizzle");

        Tilemap groundTM = PMHelper.Exist<Tilemap>(ground);
        groundTMR = PMHelper.Exist<TilemapRenderer>(ground);
        if (!groundTM)
            Assert.Fail("Level 3: There should be a <Tilemap> component on \"Ground\"'s object to use tilemaps");
        if (!groundTMR)
            Assert.Fail(
                "Level 3: There should be a <TilemapRenderer> component on \"Ground\"'s object to view created tilemaps");
        if (groundTM.size.x <= 0 || groundTM.size.y <= 0)
            Assert.Fail("Level 3: There are no added tiles to \"Ground\"'s tilemap");

        Collider2D groundCl = PMHelper.Exist<Collider2D>(ground);
        if (!groundCl || !groundCl.enabled)
        {
            Assert.Fail("Level 3: \"Ground\" object should have an assigned enabled <Collider2D> component");
        }

        if (groundCl.isTrigger)
        {
            Assert.Fail("Level 3: \"Ground\"'s <Collider2D> component should not be triggerable");
        }
    
        //Camera check
        cameraObj = GameObject.Find("Main Camera");
        if (!cameraObj)
            Assert.Fail("Level 3: There should be a camera, named \"Main Camera\" on scene");

        Camera camera = PMHelper.Exist<Camera>(cameraObj);
        if (!camera)
            Assert.Fail("Level 3: \"Main Camera\"'s object should have an attached <Camera> component");

        if (!PMHelper.CheckVisibility(camera, player.transform, 2))
            Assert.Fail("Level 3: Player's object should be in a camera view");
        if (!PMHelper.CheckVisibility(camera, background.transform, 2))
            Assert.Fail("Level 3: Background's object should be in a camera view");
    
        //Children check
        if (!PMHelper.Child(background, cameraObj))
            Assert.Fail(
                "Level 3: \"Background\"'s object should be a child of \"Main Camera\" object in order to move" +
                " camera with background, so that the background will be always in the camera view");
        if (!PMHelper.Child(ground, grid))
            Assert.Fail(
                "Level 3: \"Ground\"'s object should be a child of \"Grid\" object, because tilemap's should be a child of grid!");
   
        //Sorting check
        int layer = playerSR.sortingLayerID;
        if (backgroundSR.sortingLayerID != layer || levelEndSR.sortingLayerID != layer ||
            groundTMR.sortingLayerID != layer)
        {
            Assert.Fail("Level 3: There is no need here to create new sorting layers. " +
                        "Set all the <SpriteRenderer>s on the same sorting layer. To order visibility you should change their" +
                        "\"Order in layer\" property");
        }

        bool correctSort =
            backgroundSR.sortingOrder < levelEndSR.sortingOrder &&
            levelEndSR.sortingOrder < groundTMR.sortingOrder &&
            groundTMR.sortingOrder < playerSR.sortingOrder;
        if (!correctSort)
            Assert.Fail(
                "Level 3: Order in layers should be placed in correct order: Background < LevelEnd < Ground < Player!");
        
        //Position check
        ground.layer = LayerMask.NameToLayer("Test");
        
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            ground.layer == LayerMask.NameToLayer("Test") || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (ground.layer != LayerMask.NameToLayer("Test"))
        {
            Assert.Fail("Unexpected");
        }
        if (!PMHelper.RaycastFront2D(player.transform.position, Vector2.down,
            1 << LayerMask.NameToLayer("Test")).collider)
        {
            Assert.Fail("Level 3: Player should be placed above the ground");
        }
    }
}