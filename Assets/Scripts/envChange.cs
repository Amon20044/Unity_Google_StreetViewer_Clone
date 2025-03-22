using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material[][] skyboxes; // 2D array for floors and skyboxes

    private int currentFloor = 5;
    private int currentSkyboxIndex = 2;

    // Store last valid location
    private int lastValidFloor = 5;
    private int lastValidSkyboxIndex = 2;

    // Button prefab for 3D UI buttons
    public GameObject buttonPrefab;

    // Player reference
    public Transform player;

    // Button positions relative to the player
    private Vector3[] buttonOffsets = new Vector3[]
    {
        new Vector3(0, 0, 1),  // Forward (North)
        new Vector3(0, 0, -1), // Backward (South)
        new Vector3(-1, 0, 0), // Left (West)
        new Vector3(1, 0, 0)   // Right (East)
    };

    private GameObject[] directionButtons;

    // Declare skyboxes for each floor
    public Material skybox1, skybox2, skybox3, skybox4, skybox5, skybox6;
    void Start()
    {
        // Initialize the 2D array manually
        skyboxes = new Material[6][];
        skyboxes[0] = new Material[] { null, null, null, null, null }; // Floor 1
        skyboxes[1] = new Material[] { null, null, skybox6, null, null }; // Floor 2
        skyboxes[2] = new Material[] { null, null, skybox5, null, null }; // Floor 3
        skyboxes[3] = new Material[] { null, null, skybox3, skybox4, null }; // Floor 4
        skyboxes[4] = new Material[] { null, null, skybox2, null, null }; // Floor 5
        skyboxes[5] = new Material[] { null, null, skybox1, null, null }; // Floor 6

        // Initially apply the skybox
        ApplySkybox();

        // Create directional buttons
        CreateDirectionalButtons();
    }

    void Update()
    {
        HandleTouchInput();
        // Check input for vertical and horizontal movement
        if (Input.GetKeyDown(KeyCode.W)) MoveVertical(-1);
        else if (Input.GetKeyDown(KeyCode.S)) MoveVertical(1);
        else if (Input.GetKeyDown(KeyCode.A)) MoveHorizontal(-1);
        else if (Input.GetKeyDown(KeyCode.D)) MoveHorizontal(1);
    }

    bool IsMoveValid(int floor, int index)
    {
        return floor >= 0 && floor < skyboxes.Length &&
               index >= 0 && index < skyboxes[floor].Length &&
               skyboxes[floor][index] != null;
    }
    void CreateDirectionalButtons()
    {
        if (directionButtons != null)
        {
            foreach (var button in directionButtons)
            {
                if (button != null) Destroy(button);
            }
        }
        directionButtons = new GameObject[4];

        for (int i = 0; i < buttonOffsets.Length; i++)
        {
            bool shouldRenderButton = false;

            // Check if the next skybox is valid based on the direction
            switch (i)
            {
                case 0: // MoveForward (North)
                    shouldRenderButton = IsMoveValid(currentFloor - 1, currentSkyboxIndex);
                    break;
                case 1: // MoveBackward (South)
                    shouldRenderButton = IsMoveValid(currentFloor + 1, currentSkyboxIndex);
                    break;
                case 2: // MoveLeft (West)
                    shouldRenderButton = IsMoveValid(currentFloor, currentSkyboxIndex - 1);
                    break;
                case 3: // MoveRight (East)
                    shouldRenderButton = IsMoveValid(currentFloor, currentSkyboxIndex + 1);
                    break;
            }

            if (shouldRenderButton)
            {
                // Instantiate a button prefab
                GameObject button = Instantiate(buttonPrefab);

                // Set the button's position relative to the player
                button.transform.position = player.position + buttonOffsets[i];
                button.transform.LookAt(player); // Make the button face the player

                // Assign a label or identifier to the button
                button.name = i switch
                {
                    0 => "MoveForward",
                    1 => "MoveBackward",
                    2 => "MoveLeft",
                    3 => "MoveRight",
                    _ => "Button"
                };

                Debug.Log($"Button {button.name} created at position {button.transform.position}");

                // Store the button in the array
                directionButtons[i] = button;
            }
            else
            {
                Debug.Log($"Button for direction {i} not created because the next skybox is null.");
            }
        }
    }
    void HandleTouchInput()
    {
        if (Input.touchCount > 0) // Check if there's at least one touch
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            if (touch.phase == TouchPhase.Began) // Check if the touch just began
            {
                // Convert the touch position to a ray
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                // Perform a raycast to detect objects
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log($"Raycast hit: {hit.collider.gameObject.name}"); // Log the name of the hit object

                    // Get the parent object (if any)
                    GameObject hitObject = hit.collider.gameObject;
                    if (hitObject.transform.parent != null)
                    {
                        hitObject = hitObject.transform.parent.gameObject; // Get the parent object
                    }

                    // Check which button was hit and log the corresponding direction
                    switch (hitObject.name)
                    {
                        case "MoveForward":
                            Debug.Log("North (MoveForward) button clicked");
                            MoveVertical(-1);
                            break;
                        case "MoveBackward":
                            Debug.Log("South (MoveBackward) button clicked");
                            MoveVertical(1);
                            break;
                        case "MoveLeft":
                            Debug.Log("West (MoveLeft) button clicked");
                            MoveHorizontal(-1);
                            break;
                        case "MoveRight":
                            Debug.Log("East (MoveRight) button clicked");
                            MoveHorizontal(1);
                            break;
                        default:
                            Debug.Log($"Unknown object clicked: {hitObject.name}");
                            break;
                    }
                }
                else
                {
                    Debug.Log("Raycast did not hit any object.");
                }
            }
        }
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
                CreateDirectionalButtons();
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
            CreateDirectionalButtons();
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
                // Use the SkyboxTransition script for smooth transitions
                SkyboxTransition transition = GetComponent<SkyboxTransition>();
                if (transition != null)
                {
                    transition.StartSkyboxTransition(selectedSkybox);
                }
                else
                {
                    // Fallback to direct assignment if the transition script is missing
                    RenderSettings.skybox = selectedSkybox;
                    DynamicGI.UpdateEnvironment();
                }
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
}