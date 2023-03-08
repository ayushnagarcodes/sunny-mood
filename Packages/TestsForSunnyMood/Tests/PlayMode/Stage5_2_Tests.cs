using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.SceneManagement;

[Description(""), Category("5")]
public class Stage5_2_Tests
{
    private GameObject player, ground, levelEnd;
    private Transform playerT;
    private Collider2D playerCl, groundCl, levelEndCl;
    private Collider2D leftBoundCl, rightBoundCl;
    private Rigidbody2D playerRB;
    private Vector3 jumpPlace;
    private Vector2 leftBoundPoint;
    private Vector2 rightBoundPoint;

    [UnityTest, Order(0)]
    public IEnumerator NotMovingWithoutInputCheck()
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
        playerT = player.transform;
        playerCl = PMHelper.Exist<Collider2D>(player);

        ground = GameObject.Find("Ground");
        groundCl = PMHelper.Exist<Collider2D>(ground);

        Time.timeScale = 10;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(groundCl) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if ((Time.unscaledTime - start) * Time.timeScale >= 5)
        {
            Assert.Fail(
                "Level 3: In some time after the scene was loaded \"Player\"'s collider should be touching \"Ground\"'s collider");
        }

        yield return new WaitForSeconds(1);
        Vector3 startPos = playerT.position;
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            startPos != playerT.position || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (startPos != playerT.position)
        {
            Assert.Fail("Level 3: \"Player\"'s position should not change if there were no input provided");
        }
    }

    [UnityTest, Order(1)]
    public IEnumerator MovementLeftCheck()
    {
        Vector3 posStart = playerT.position;
        VInput.KeyDown(KeyCode.A);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            posStart != playerT.position || (Time.unscaledTime - start) * Time.timeScale > 3);
        if (posStart == playerT.position)
        {
            Assert.Fail("Level 3: When the A-key is pressed \"Player\"'s position should change");
        }
        VInput.KeyUp(KeyCode.A);
        Vector3 posEnd = player.transform.position;
        
        if (posEnd.x >= posStart.x)
        {
            Assert.Fail("Level 3: When the A-key is pressed X-axis of \"Player\"'s object should decrease");
        }
        if (Mathf.Abs(posEnd.y - posStart.y)>=0.01f || Mathf.Abs(posEnd.z - posStart.z)>=0.01f)
        {
            Assert.Fail("Level 3: When the A-key is pressed y-axis and z-axis should not change");
        }
    }

    [UnityTest, Order(2)]
    public IEnumerator MovementRightCheck()
    {
        Vector3 posStart = playerT.position;
        VInput.KeyDown(KeyCode.D);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            posStart != playerT.position || (Time.unscaledTime - start) * Time.timeScale > 3);
        if (posStart == playerT.position)
        {
            Assert.Fail("Level 3: When the D-key is pressed \"Player\"'s position should change");
        }
        VInput.KeyUp(KeyCode.D);
        Vector3 posEnd = player.transform.position;
        
        if (posEnd.x <= posStart.x)
        {
            Assert.Fail("Level 3: When the D-key is pressed X-axis of \"Player\"'s object should increase");
        }
        if (Mathf.Abs(posEnd.y - posStart.y)>=0.01f || Mathf.Abs(posEnd.z - posStart.z)>=0.01f)
        {
            Assert.Fail("Level 3: When the D-key is pressed y-axis and z-axis should not change");
        }
    }

    [UnityTest, Order(3)]
    public IEnumerator CorrectMovement()
    {
        playerRB = PMHelper.Exist<Rigidbody2D>(player);
        playerRB.constraints = RigidbodyConstraints2D.FreezeAll;
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.constraints == RigidbodyConstraints2D.FreezeAll || (Time.unscaledTime - start) * Time.timeScale > 1);
        Vector3 posStart = playerT.position;
        
        VInput.KeyDown(KeyCode.D);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            posStart != playerT.position || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (posStart != playerT.position)
        {
            Assert.Fail("Level 3: \"Player\"'s movement should be implemented via <Rigidbody2D> component");
        }
        VInput.KeyUp(KeyCode.D);

        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.constraints == RigidbodyConstraints2D.FreezeRotation || (Time.unscaledTime - start) * Time.timeScale > 2);
        
        VInput.KeyDown(KeyCode.D);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.velocity.x != 0 || (Time.unscaledTime - start) * Time.timeScale > 3);
        if (playerRB.velocity.x == 0)
        {
            Assert.Fail("Level 3: \"Player\"'s horizontal movement should be implemented by changing x-axis velocity");
        }
        VInput.KeyUp(KeyCode.D);
    }

    [UnityTest, Order(4)]
    public IEnumerator JumpCheck()
    {
        VInput.KeyPress(KeyCode.Space);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.velocity.y > 0 || (Time.unscaledTime - start) * Time.timeScale > 3);
        if (playerRB.velocity.y <= 0)
        {
            Assert.Fail("Level 3: When the Space-button was pressed jump should be provided, so the Rigidbody2D's " +
                        "y-axis velocity should increase");
        }

        start = Time.unscaledTime;
        float startOfJump = start;
        yield return new WaitUntil(() =>
            playerRB.velocity.y < 0 || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (playerRB.velocity.y >= 0)
        {
            Assert.Fail(
                "Level 3: Y-axis velocity of <Rigidbody2D> component should decrease after the jump was provided," +
                " because of the gravity, and after some time it should become negative");
        }

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(groundCl) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (!playerCl.IsTouching(groundCl))
        {
            Assert.Fail("Level 3: After the jump is provided, player should fall down to the ground. " +
                        "Jump duration should be less than 2 seconds");
        }

        float duration = (Time.unscaledTime - startOfJump) * Time.timeScale;
        if (duration >= 3)
        {
            Assert.Fail("Level 3: Jump duration should be less than 2 seconds, but in your case it's " + duration);
        }
    }

    [UnityTest, Order(5)]
    public IEnumerator JumpInAirCheck()
    {
        Time.timeScale = 1;

        VInput.KeyPress(KeyCode.Space);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.velocity.y < 0 || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (playerRB.velocity.y >= 0)
        {
            Assert.Fail(
                "Level 3: Y-axis velocity of <Rigidbody2D> component should decrease after the jump was provided," +
                " because of the gravity, and after some time it should become negative");
        }

        jumpPlace = playerT.position;

        float velocityPrev = playerRB.velocity.y;
        VInput.KeyPress(KeyCode.Space);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerRB.velocity.y > velocityPrev && playerRB.velocity.y!=0 || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (playerRB.velocity.y > velocityPrev && playerRB.velocity.y!=0)
        {
            Assert.Fail("Level 3: \"Player\" should not be able to jump while it's in midair");
        }
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(groundCl) || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (!playerCl.IsTouching(groundCl))
        {
            Assert.Fail("Level 3: After the jump is provided, player should fall down to the ground. " +
                        "Jump duration should be less than 2 seconds");
        }
    }

    [UnityTest, Order(6)]
    public IEnumerator CheckBounds()
    {
        if (!PMHelper.CheckTagExistance("Bounds"))
        {
            Assert.Fail("Level 3: \"Bounds\" tag was not added to project");
        }

        GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");
        if (bounds.Length != 2)
        {
            Assert.Fail("Level 3: There should be 2 game objects with \"Bounds\" tag on scene");
        }

        foreach (GameObject b in bounds)
        {
            SpriteRenderer boundsSR = PMHelper.Exist<SpriteRenderer>(b);
            if (boundsSR)
                Assert.Fail(
                    "Level 3: There should be no <SpriteRenderer> component on \"Bounds\" objects in order to make them non-visible");
            Collider2D boundsCL = PMHelper.Exist<Collider2D>(b);
            if (!boundsCL || !boundsCL)
            {
                Assert.Fail("Level 3: \"Bounds\" objects should have an assigned enabled <Collider2D> component");
            }

            if (boundsCL.isTrigger)
            {
                Assert.Fail("Level 3: \"Bounds\"' <Collider2D> component should not be triggerable");
            }

            if (!boundsCL.sharedMaterial || boundsCL.sharedMaterial.friction != 0)
            {
                Assert.Fail(
                    "Level 3: \"Bounds\"' <Collider2D> component should have assigned <2D Physics Material> with " +
                    "friction set to zero, so the player won't be able to hang on the boundaries");
            }

            b.layer = LayerMask.NameToLayer("Test");
            float start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                b.layer == LayerMask.NameToLayer("Test") || (Time.unscaledTime - start) * Time.timeScale > 1);
        }

        Vector3 position = playerT.position;
        
        leftBoundCl = PMHelper.RaycastFront2D(position, Vector2.left,
            1 << LayerMask.NameToLayer("Test")).collider;
        rightBoundCl = PMHelper.RaycastFront2D(position, Vector2.right,
            1 << LayerMask.NameToLayer("Test")).collider;

        leftBoundPoint = PMHelper.RaycastFront2D(position, Vector2.left,
            1 << LayerMask.NameToLayer("Test")).point;
        rightBoundPoint = PMHelper.RaycastFront2D(position, Vector2.right,
            1 << LayerMask.NameToLayer("Test")).point;

        if (!leftBoundCl)
        {
            Assert.Fail("Level 3: There should be an object with \"Bounds\" tag on \"Player\"'s left and stop it from" +
                        " passing through (\"Player\" should not be able to jump it over, or crawl underneath it)");
        }

        if (!rightBoundCl)
        {
            Assert.Fail(
                "Level 3: There should be an object with \"Bounds\" tag on \"Player\"'s right and stop it from" +
                " passing through (\"Player\" should not be able to jump it over, or crawl underneath it)");
        }

        if (!PMHelper.RaycastFront2D(jumpPlace, Vector2.left,
            1 << LayerMask.NameToLayer("Test")).collider)
        {
            Assert.Fail("Level 3: There should be an object with \"Bounds\" tag on \"Player\"'s left and stop it from" +
                        " passing through (\"Player\" should not be able to jump it over, or crawl underneath it)");
        }

        if (!PMHelper.RaycastFront2D(jumpPlace, Vector2.right,
            1 << LayerMask.NameToLayer("Test")).collider)
        {
            Assert.Fail(
                "Level 3: There should be an object with \"Bounds\" tag on \"Player\"'s right and stop it from" +
                " passing through (\"Player\" should not be able to jump it over, or crawl underneath it)");
        }

        foreach (GameObject b in bounds)
        {
            b.layer = LayerMask.NameToLayer("Default");
        }

        ground.layer = LayerMask.NameToLayer("Test");

        if (!PMHelper.RaycastFront2D(leftBoundPoint, Vector2.down,
            1 << LayerMask.NameToLayer("Test")).collider)
        {
            Assert.Fail(
                "Level 3: Left boundary should be placed above or 'touching' the ground object, so \"Player\" won't" +
                " be able to fall down to the void");
        }

        if (!PMHelper.RaycastFront2D(rightBoundPoint, Vector2.down,
            1 << LayerMask.NameToLayer("Test")).collider)
        {
            Assert.Fail(
                "Level 3: Right boundary should be placed above or 'touching' the ground object, so \"Player\" won't" +
                " be able to fall down to the void");
        }
    }

    [UnityTest, Order(7)]
    public IEnumerator CameraMovementCheck()
    {
        GameObject cameraObj = GameObject.Find("Main Camera");
        Camera camera = PMHelper.Exist<Camera>(cameraObj);

        Time.timeScale = 15;
        VInput.KeyDown(KeyCode.A);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(leftBoundCl) || (Time.unscaledTime - start) * Time.timeScale > 12);
        if (!playerCl.IsTouching(leftBoundCl))
        {
            Assert.Fail(
                "Level 3: Player should be able to get from left bound to the right one in less than 10 game-time seconds");
        }
        VInput.KeyUp(KeyCode.A);
        
        if (!PMHelper.CheckVisibility(camera, playerT, 2))
        {
            Assert.Fail("Level 3: Player should always stay in a camera view");
        }

        
        VInput.KeyDown(KeyCode.D);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerCl.IsTouching(rightBoundCl) || (Time.unscaledTime - start) * Time.timeScale > 12);
        if (!playerCl.IsTouching(rightBoundCl))
        {
            Assert.Fail(
                "Level 3: Player should be able to get from left bound to the right one in less than 10 game-time seconds");
        }
        VInput.KeyUp(KeyCode.D);

        if (!PMHelper.CheckVisibility(camera, playerT, 2))
        {
            Assert.Fail("Level 3: Player should always stay in a camera view");
        }
    }

    [UnityTest, Order(8)]
    public IEnumerator LevelEndCheck()
    {
        Vector2 LevelEndPlace = (Vector2) levelEnd.transform.position + levelEndCl.offset;

        if (!PMHelper.RaycastFront2D(LevelEndPlace, Vector2.down,
            1 << LayerMask.NameToLayer("Test")).collider)
        {
            Assert.Fail(
                "Level 3: \"LevelEnd\" should be placed above or 'touching' the ground object, so \"Player\" could reach it");
        }

        if (LevelEndPlace.x <= leftBoundPoint.x || LevelEndPlace.x >= rightBoundPoint.x)
        {
            Assert.Fail("Level 3: \"LevelEnd\" should be placed between boundaries, so \"Player\" could reach it");
        }

        if (LevelEndPlace.y >= jumpPlace.y)
        {
            Assert.Fail("Level 3: \"LevelEnd\" should not be placed too high, so \"Player\" could reach it");
        }
    
        Scene cur = SceneManager.GetActiveScene();
        playerT.position = LevelEndPlace;

        levelEndCl.enabled = true;
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene() != cur || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (SceneManager.GetActiveScene() == cur)
        {
            Assert.Fail(
                "Level 3: When \"Player\" collides with a \"LevelEnd\" object - scene should change (or just reload).");
        }
    }
}