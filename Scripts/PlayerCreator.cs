using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCreator : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        var player1 = PlayerInput.Instantiate(playerPrefab, controlScheme: "KeyboardLeft", pairWithDevice: Keyboard.current);
        player1.SwitchCurrentControlScheme("KeyboardLeft", Keyboard.current);
        var player2 = PlayerInput.Instantiate(playerPrefab, controlScheme: "KeyboardRight", pairWithDevice: Keyboard.current);
        player2.SwitchCurrentControlScheme("KeyboardRight", Keyboard.current);
    }
}
