using Il2CppSystem.Collections.Generic;
using SOD.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInfoProvider
{
    FirstPersonController playerObject = GameObject.FindObjectOfType<FirstPersonController>();
    public Vector3Int GetPlayerLocation()
    {
        if (playerObject != null)
        {
            Vector3 playerPosition = playerObject.transform.position;
            return new Vector3Int(Mathf.RoundToInt(playerPosition.x),
                                   Mathf.RoundToInt(playerPosition.y),
                                   Mathf.RoundToInt(playerPosition.z));
        }
        return new Vector3Int(0, 0, 0);
    }
    public void SetPlayerLocation(Vector3 loc)
    {
        playerObject.transform.position = loc;
    }
    public NewNode GetPlayerNode()
    {
        Player player = Player.Instance;
        return player.currentNode;
    }
    public bool GetIsRunning()
    {
        Player player = Player.Instance;
        return player.isRunning;
    }
    public void SetIsRunning(bool isRunning)
    {
        Player player = Player.Instance;
        player.isRunning = isRunning;
    }
    public bool GetHasJumped()
    {
        FirstPersonController control = Object.FindObjectOfType<FirstPersonController>();
        return control.m_Jump;
    }
    public bool GetIsJumping()
    {
        FirstPersonController control = Object.FindObjectOfType<FirstPersonController>();
        return control.m_Jumping;
    }
    public void SetIsJumping(bool jump)
    {
        FirstPersonController control = Object.FindObjectOfType<FirstPersonController>();
        control.m_Jumping = jump;
    }
    public bool GetIsGrounded()
    {
        Player player = Player.Instance;
        return player.isGrounded;
    }
    public void SetIsGrounded(bool grounded)
    {
        Player player = Player.Instance;
        player.isGrounded = grounded;
    }
    public void AddPoisoned(float amount, Human who)
    {
        Player player = Player.Instance;
        player.AddPoisoned(amount, player);
    }
    public float GetMovementRunSpeed()
    {
        GameplayControls player = GameplayControls.Instance;
        return player.playerRunSpeed;
    }
    public void SetMovementRunSpeed(float setMovementRunSpeed)
    {
        GameplayControls player = GameplayControls.Instance;
        player.playerRunSpeed = setMovementRunSpeed;
    }
    public float GetMovementWalkSpeed()
    {
        GameplayControls player = GameplayControls.Instance;
        return player.playerWalkSpeed;
    }
    public void SetMovementWalkSpeed(float setMovementWalkSpeed)
    {
        GameplayControls player = GameplayControls.Instance;
        player.playerWalkSpeed = setMovementWalkSpeed;
    }
    public float GetCurrentHealth()
    {
        Player player = Player.Instance;
        return player.currentHealth;
    }
    public void SetCurrentHealth(float health)
    {
        Player player = Player.Instance;
        player.currentHealth = health;
    }
    public string GetPassword()
    {
        Player player = Player.Instance;
        Il2CppSystem.Collections.Generic.List<int> digits = player.passcode.digits;
        System.Text.StringBuilder passcodeBuilder = new System.Text.StringBuilder();
        for (int i = 0; i < digits.Count; i++)
        {
            passcodeBuilder.Append(digits[i].ToString());
        }
        string passcodeString = passcodeBuilder.ToString();
        return passcodeString;
    }
    public void SetPassword(string passcode)
    {
        Player player = Player.Instance;

        player.passcode.digits.Clear();

        foreach (char c in passcode)
        {
            if (char.IsDigit(c))
            {
                player.passcode.digits.Add(int.Parse(c.ToString()));
            }
            else
            {
                return;
            }
        }
    }
    public bool IsOnDuty()
    {
        Player player = Player.Instance;
        return player.isOnDuty;
    }
    public bool IsCurrentlyInAutoTravel()
    {
        Player player = Player.Instance;
        return player.autoTravelActive;
    }
    public float GetBlackEye()
    {
        Player player = Player.Instance;
        return player.blackEye;
    }
    public void SetBlackEye(float amount)
    {
        Player player = Player.Instance;
        player.blackEye = amount;
    }
    public float GetBlackedOut()
    {
        Player player = Player.Instance;
        return player.blackedOut;
    }
    public void SetBlackedOut(float amount)
    {
        Player player = Player.Instance;
        player.blackedOut = amount;
    }
    public float GetBleeding()
    {
        Player player = Player.Instance;
        return player.bleeding;
    }
    public void SetBleeding(float amount)
    {
        Player player = Player.Instance;
        player.bleeding = amount;
    }
    public void AddBleeding(float amount)
    {
        Player player = Player.Instance;
        player.AddBleeding(amount);
    }
    public void AddDrunk(float amount)
    {
        Player player = Player.Instance;
        player.AddDrunk(amount);
    }
    public void AddBruised(float amount)
    {
        Player player = Player.Instance;
        player.AddBruised(amount);
    }
    public Telephone GetAnsweringPhone()
    {
        Player player = Player.Instance;
        return player.answeringPhone;
    }
    public Il2CppSystem.Collections.Generic.List<NewAddress> ApartmentsOwned()
    {
        Player player = Player.Instance;
        return player.apartmentsOwned;
    }
}