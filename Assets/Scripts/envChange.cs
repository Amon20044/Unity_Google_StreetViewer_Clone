using UnityEngine;
using UnityEngine.UI;  // Import for UI elements

public class SkyboxChanger : MonoBehaviour
{
    public Material[][] skyboxes; // 2D array for floors and skyboxes

    private int currentFloor =5;
    private int currentSkyboxIndex = 2;

    // Store last valid location
    private int lastValidFloor = 5;
    private int lastValidSkyboxIndex = 2;

    // UI Buttons for the arrows
    public Button upArrow, downArrow, leftArrow, rightArrow;

    // Declare skyboxes for each floor
    public Material skybox1, skybox2, skybox3, skybox4, skybox5, skybox6;

    void Start()
    {
        // Initialize the 2D array manually
        skyboxes = new Material[6][];
        skyboxes[0] = new Material[] { null, null, skybox6, null ,null}; // Floor 1
        skyboxes[1] = new Material[] { null, null, skybox5, null, null }; // Floor 2
        skyboxes[2] = new Material[] { null, null, skybox4, null, null }; // Floor 3
        skyboxes[3] = new Material[] { null, null, skybox3, null, null }; // Floor 4
        skyboxes[4] = new Material[] { null, null, skybox2, null, null }; // Floor 5
        skyboxes[5] = new Material[] { null, null, skybox1, null, null }; // Floor 6

        // Initially apply the skybox
        ApplySkybox();

        // Check available directions
        UpdateArrowVisibility();
    }

    void Update()
    {
        // Check input for vertical and horizontal movement
        if (Input.GetKeyDown(KeyCode.W)) MoveVertical(-1);
        else if (Input.GetKeyDown(KeyCode.S)) MoveVertical(1);
        else if (Input.GetKeyDown(KeyCode.A)) MoveHorizontal(-1);
        else if (Input.GetKeyDown(KeyCode.D)) MoveHorizontal(1);
    }

    public void MoveVertical(int direction)
{
    Debug.Log($"MoveVertical called with direction: {direction}");

    int newFloor = currentFloor + direction;
    if (newFloor >= 0 && newFloor < skyboxes.Length)
    {
        int tempSkyboxIndex = Mathf.Clamp(currentSkyboxIndex, 0, skyboxes[newFloor].Length - 1);

        if (skyboxes[newFloor][tempSkyboxIndex] != null)
        {
            currentFloor = newFloor;
            currentSkyboxIndex = tempSkyboxIndex;
            SaveLastValidLocation();
            ApplySkybox();
            Debug.Log($"Skybox changed to Floor: {currentFloor}, Index: {currentSkyboxIndex}");
        }
        else
        {
            Debug.Log("Invalid move! No skybox available.");
            RevertToLastValidLocation();
        }
    }
}


    public void MoveHorizontal(int direction)
    {
        int newIndex = currentSkyboxIndex + direction;
        if (newIndex >= 0 && newIndex < skyboxes[currentFloor].Length && skyboxes[currentFloor][newIndex] != null)
        {
            currentSkyboxIndex = newIndex;
            SaveLastValidLocation();
            ApplySkybox();
        }
        else
        {
            RevertToLastValidLocation();
        }
    }

    void ApplySkybox()
    {
        if (currentFloor >= 0 && currentFloor < skyboxes.Length &&
            currentSkyboxIndex >= 0 && currentSkyboxIndex < skyboxes[currentFloor].Length)
        {
            Material selectedSkybox = skyboxes[currentFloor][currentSkyboxIndex];

            if (selectedSkybox != null)
            {
                RenderSettings.skybox = selectedSkybox;
                DynamicGI.UpdateEnvironment();
                UpdateArrowVisibility();
            }
            else
            {
                RevertToLastValidLocation();
            }
        }
    }

    void SaveLastValidLocation()
    {
        lastValidFloor = currentFloor;
        lastValidSkyboxIndex = currentSkyboxIndex;
    }

    void RevertToLastValidLocation()
    {
        currentFloor = lastValidFloor;
        currentSkyboxIndex = lastValidSkyboxIndex;
        Debug.Log("Invalid location! Returning to last valid skybox.");
    }

    void UpdateArrowVisibility()
    {
        if (upArrow != null) upArrow.gameObject.SetActive(IsMoveValid(currentFloor - 1, currentSkyboxIndex));
        if (downArrow != null) downArrow.gameObject.SetActive(IsMoveValid(currentFloor + 1, currentSkyboxIndex));
        if (leftArrow != null) leftArrow.gameObject.SetActive(IsMoveValid(currentFloor, currentSkyboxIndex - 1));
        if (rightArrow != null) rightArrow.gameObject.SetActive(IsMoveValid(currentFloor, currentSkyboxIndex + 1));
    }

    bool IsMoveValid(int floor, int index)
    {
        return (floor >= 0 && floor < skyboxes.Length) &&
               (index >= 0 && index < skyboxes[floor].Length) &&
               (skyboxes[floor][index] != null);
    }
}
