using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[Description("Sunshine is the best medicine."), Category("6")]
public class Stage6_Tests
{
    private GameObject canvas, chooselevel, play, exit, panel, close, level1, level2, level3;
    private Button chooseLevelButton, playButton, exitButton, closeButton, level1Button, level2Button, level3Button;

    [UnityTest, Order(0)]
    public IEnumerator SetUp()
    {
        PlayerPrefs.DeleteAll();

        if (!Application.CanStreamedLevelBeLoaded("Main Menu"))
        {
            Assert.Fail("\"Main Menu\" scene is misspelled or was not added to build settings");
        }

        SceneManager.LoadScene("Main Menu");
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Main Menu" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            Assert.Fail("\"Main Menu\" scene can't be loaded");
        }
    }

    [UnityTest, Order(1)]
    public IEnumerator ChooseLevelNecessary()
    {
        canvas = GameObject.Find("Canvas");
        if (!canvas)
            Assert.Fail("There should be canvas on scene named \"Canvas\"");

        RectTransform canvasRT = PMHelper.Exist<RectTransform>(canvas);
        Canvas canvasCV = PMHelper.Exist<Canvas>(canvas);
        CanvasScaler canvasCS = PMHelper.Exist<CanvasScaler>(canvas);
        GraphicRaycaster canvasGR = PMHelper.Exist<GraphicRaycaster>(canvas);

        if (!canvasRT)
            Assert.Fail("There should be <RectTransform> component on \"Canvas\" object");
        if (!canvasCV || !canvasCV.enabled)
            Assert.Fail("There should be enabled <Canvas> component on \"Canvas\" object");
        if (!canvasCS || !canvasCS.enabled)
            Assert.Fail("There should be enabled <CanvasScaler> component on \"Canvas\" object");
        if (!canvasGR || !canvasGR.enabled)
            Assert.Fail("There should be enabled <GraphicRaycaster> component on \"Canvas\" object");

        chooselevel = GameObject.Find("ChooseLevel");
        play = GameObject.Find("Play");
        exit = GameObject.Find("Exit");

        foreach ((GameObject, string) testCase in new[]
        {
            (chooselevel, "ChooseLevel"),
            (play, "Play"),
            (exit, "Exit")
        })
        {
            if (!testCase.Item1)
                Assert.Fail("There should be button on scene named \"" + testCase.Item2 + "\"");

            RectTransform rt = PMHelper.Exist<RectTransform>(testCase.Item1);
            if (!rt)
                Assert.Fail("There should be <RectTransform> component on \"" + testCase.Item2 + "\" object");
            if (!PMHelper.CheckRectTransform(rt))
            {
                Assert.Fail("Anchors of \"" + testCase.Item2 +
                            "\"'s <RectTransform> component are incorrect or it's offsets " +
                            "are not equal to zero, might be troubles with different resolutions");
            }

            Button b = PMHelper.Exist<Button>(testCase.Item1);
            if (!b || !b.enabled)
                Assert.Fail("There should be <Button> component on \"" + testCase.Item2 + "\" object");
        }

        chooseLevelButton = PMHelper.Exist<Button>(chooselevel);
        playButton = PMHelper.Exist<Button>(play);
        exitButton = PMHelper.Exist<Button>(exit);

        panel = GameObject.Find("Panel");
        if (panel)
            Assert.Fail("The \"Panel\" object should be hidden on scene by default");

        chooseLevelButton.onClick.Invoke();

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.Find("Panel") || (Time.unscaledTime - start) * Time.timeScale > 1);
        panel = GameObject.Find("Panel");
        if (!panel)
            Assert.Fail("There is no \"Panel\" object after pressing \"ChooseLevel\" button");

        //Check children
        foreach (var g in new[] {panel, play, exit, chooselevel})
        {
            if (!PMHelper.Child(g, canvas))
                Assert.Fail("\"" + g.name + "\" object should be a child of \"Canvas\" object");
        }

        //Check panel objects
        RectTransform panelRT = PMHelper.Exist<RectTransform>(panel);

        if (!panelRT)
            Assert.Fail("There should be <RectTransform> component on \"Panel\" object");
        if (!PMHelper.CheckRectTransform(panelRT))
        {
            Assert.Fail("Anchors of \"Panel\"'s <RectTransform> component are incorrect or it's offsets " +
                        "are not equal to zero, might be troubles with different resolutions");
        }

        close = GameObject.Find("Close");
        level1 = GameObject.Find("Level 1");
        level2 = GameObject.Find("Level 2");
        level3 = GameObject.Find("Level 3");

        foreach ((GameObject, string) testCase in new[]
        {
            (close, "Close"),
            (level1, "Level 1"),
            (level2, "Level 2"),
            (level3, "Level 3"),
        })
        {
            if (!testCase.Item1)
                Assert.Fail("There should be button on scene after pressing \"ChooseLevel\" button, named \"" +
                            testCase.Item2 + "\"");

            RectTransform rt = PMHelper.Exist<RectTransform>(testCase.Item1);
            if (!rt)
                Assert.Fail("There should be <RectTransform> component on \"" + testCase.Item2 + "\" object");
            if (!PMHelper.CheckRectTransform(rt))
            {
                Assert.Fail("Anchors of \"" + testCase.Item2 +
                            "\"'s <RectTransform> component are incorrect or it's offsets " +
                            "are not equal to zero, might be troubles with different resolutions");
            }

            Button b = PMHelper.Exist<Button>(testCase.Item1);
            if (!b || !b.enabled)
                Assert.Fail("There should be <Button> component on \"" + testCase.Item2 + "\" object");

            if (!PMHelper.Child(testCase.Item1, panel))
                Assert.Fail("\"" + testCase.Item2 + "\" object should be a child of \"Panel\" object");
        }

        closeButton = PMHelper.Exist<Button>(close);
        level1Button = PMHelper.Exist<Button>(level1);
        level2Button = PMHelper.Exist<Button>(level2);
        level3Button = PMHelper.Exist<Button>(level3);
    }

    [UnityTest, Order(3)]
    public IEnumerator PlayTest()
    {
        Time.timeScale = 3;

        if (!level1Button.interactable || level2Button.interactable || level3Button.interactable)
        {
            Assert.Fail(
                "When the game was started by the first time - player should be able to choose only first level," +
                " others should not be interactable");
        }

        closeButton.onClick.Invoke();
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !GameObject.Find("Panel") || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (GameObject.Find("Panel"))
            Assert.Fail("After pressing \"Close\" button - \"Panel\" object should become non active");

        playButton.onClick.Invoke();
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name.Equals("Level 1") || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (!SceneManager.GetActiveScene().name.Equals("Level 1"))
        {
            Assert.Fail("When player started a game first time - \"Play\" button should transfer him to \"Level 1\"");
        }

        VInput.KeyPress(KeyCode.Escape);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name.Equals("Main Menu") || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (!SceneManager.GetActiveScene().name.Equals("Main Menu"))
        {
            Assert.Fail("When any level is loaded - Escape key should transfer player to \"Main Menu\"");
        }

        void CheckLevels(bool i1, bool i2, bool i3)
        {
            level1 = GameObject.Find("Level 1");
            level2 = GameObject.Find("Level 2");
            level3 = GameObject.Find("Level 3");
            if (!level1 || !level2 || !level3)
            {
                Assert.Fail("To disable buttons there is no need to deactivate it's object, you should just uncheck " +
                            "it's <Button>'s interactable property");
            }

            level1Button = PMHelper.Exist<Button>(level1);
            level2Button = PMHelper.Exist<Button>(level2);
            level3Button = PMHelper.Exist<Button>(level3);
            if (!level1Button || !level2Button || !level3Button)
            {
                Assert.Fail(
                    "To disable buttons there is no need to disable it's <Button> component, you should just uncheck " +
                    "<Button>'s interactable property");
            }

            if (level1Button.interactable != i1 || level2Button.interactable != i2 || level3Button.interactable != i3)
            {
                Assert.Fail("From \"Main Menu\"'s ChooseLevel panel, player should be able to choose between levels," +
                            " that he had already passed and the one, that is next, others should be non-interactable");
            }
        }

        GameObject.Find("ChooseLevel").GetComponent<Button>().onClick.Invoke();
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.Find("Panel") || (Time.unscaledTime - start) * Time.timeScale > 1);

        CheckLevels(true, false, false);
        for (int i = 1; i <= 2; i++)
        {
            GameObject.Find("Close").GetComponent<Button>().onClick.Invoke();
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                !GameObject.Find("Panel") || (Time.unscaledTime - start) * Time.timeScale > 1);

            GameObject.Find("Play").GetComponent<Button>().onClick.Invoke();
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                SceneManager.GetActiveScene().name.Equals("Level " + i) ||
                (Time.unscaledTime - start) * Time.timeScale > 1);
            if (!SceneManager.GetActiveScene().name.Equals("Level " + i))
            {
                Assert.Fail("When player left level without having it passed," +
                            " \"Play\" button should transfer him to same level");
            }

            GameObject.Find("Player").transform.position =
                GameObject.Find("LevelEnd").GetComponent<Collider2D>().bounds.center;

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                SceneManager.GetActiveScene().name.Equals("Level " + (i + 1)) ||
                (Time.unscaledTime - start) * Time.timeScale > 1);
            if (!SceneManager.GetActiveScene().name.Equals("Level " + (i + 1)))
            {
                Assert.Fail("After passing level " + i + ", level " + (i + 1) + " should be loaded");
            }

            VInput.KeyPress(KeyCode.Escape);
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                SceneManager.GetActiveScene().name.Equals("Main Menu") ||
                (Time.unscaledTime - start) * Time.timeScale > 1);
            if (!SceneManager.GetActiveScene().name.Equals("Main Menu"))
            {
                Assert.Fail("When any level is loaded - Escape key should transfer player to \"Main Menu\"");
            }

            GameObject.Find("ChooseLevel").GetComponent<Button>().onClick.Invoke();
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                GameObject.Find("Panel") || (Time.unscaledTime - start) * Time.timeScale > 1);

            CheckLevels(true, true, i == 2);
        }

        for (int i = 1; i <= 3; i++)
        {
            GameObject.Find("Level " + i).GetComponent<Button>().onClick.Invoke();

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                SceneManager.GetActiveScene().name.Equals("Level " + i) ||
                (Time.unscaledTime - start) * Time.timeScale > 1);
            if (!SceneManager.GetActiveScene().name.Equals("Level " + i))
            {
                Assert.Fail("\"Level " + i + "\" button should transfer player to \"Level " + i + "\" scene");
            }

            VInput.KeyPress(KeyCode.Escape);
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                SceneManager.GetActiveScene().name.Equals("Main Menu") ||
                (Time.unscaledTime - start) * Time.timeScale > 1);
            if (!SceneManager.GetActiveScene().name.Equals("Main Menu"))
            {
                Assert.Fail("When any level is loaded - Escape key should transfer player to \"Main Menu\"");
            }

            GameObject.Find("ChooseLevel").GetComponent<Button>().onClick.Invoke();
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                GameObject.Find("Panel") || (Time.unscaledTime - start) * Time.timeScale > 1);

            CheckLevels(true, true, true);
        }
    }
}