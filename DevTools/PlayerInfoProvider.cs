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
    public void SetPlayerNode(Vector3Int loc)
    {
        Player player = Player.Instance;
        player.currentNodeCoord = loc;
    }
    public NewNode GetPlayerNode()
    {
        Player player = Player.Instance;
        return player.currentNode;
    }
    public Vector3 GetPlayerNodeCoord()
    {
        Player player = Player.Instance;
        return player.currentNodeCoord;
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

    public float GetDrunk()
    {
        Player player = Player.Instance;
        return player.drunk;
    }

    public void SetDrunk(float amount)
    {
        Player player = Player.Instance;
        player.drunk = amount;
    }

    public float GetBruised()
    {
        Player player = Player.Instance;
        return player.bruised;
    }

    public void SetBruised(float amount)
    {
        Player player = Player.Instance;
        player.bruised = amount;
    }

    public float GetPoisoned()
    {
        Player player = Player.Instance;
        return player.poisoned;
    }

    public void SetPoisoned(float amount)
    {
        Player player = Player.Instance;
        player.poisoned = amount;
    }
    public bool GetIsDead()
    {
        Player player = Player.Instance;
        return player.isDead;
    }

    public void AddHealth(float amount)
    {
        Player player = Player.Instance;
        player.AddHealth(amount);
    }

    public void EndAutoTravel()
    {
        Player player = Player.Instance;
        player.EndAutoTravel();
    }
    public void AddSick(float amount)
    {
        Player player = Player.Instance;
        player.AddSick(amount);
    }
    
    public void AddHeadache(float amount)
    {
        Player player = Player.Instance;
        player.AddHeadache(amount);
    }

    public void AddWet(float amount)
    {
        Player player = Player.Instance;
        player.AddWet(amount);
    }

    public void AddBrokenLeg(float amount)
    {
        Player player = Player.Instance;
        player.AddBrokenLeg(amount);
    }

    public void AddNumb(float amount)
    {
        Player player = Player.Instance;
        player.AddNumb(amount);
    }

    public void KillPlayer()
    {
        Player player = Player.Instance;
        player.KillPlayer();
    }

    public void Trip(float damage, bool forwards = false, bool playSound = true)
    {
        Player player = Player.Instance;
        player.Trip(damage, forwards, playSound);
    }

    public void SetMaxSpeed(float walkSpeed, float runSpeed)
    {
        Player player = Player.Instance;
        player.SetMaxSpeed(walkSpeed, runSpeed);
    }

    public void EnablePlayerMovement(bool val, bool updateCulling = true)
    {
        Player player = Player.Instance;
        player.EnablePlayerMovement(val, updateCulling);
    }

    public void EnablePlayerMouseLook(bool val, bool forceHideMouseOnDisable = false)
    {
        Player player = Player.Instance;
        player.EnablePlayerMouseLook(val, forceHideMouseOnDisable);
    }

    public void EnableCharacterController(bool val)
    {
        Player player = Player.Instance;
        player.EnableCharacterController(val);
    }

    public void SetLockpickingState(bool val)
    {
        Player player = Player.Instance;
        player.SetLockpickingState(val);
    }

    public void AddNourishment(float amount)
    {
        Player player = Player.Instance;
        player.AddNourishment(amount);
    }

    public void AddHydration(float amount)
    {
        Player player = Player.Instance;
        player.AddHydration(amount);
    }

    public void AddEnergy(float amount)
    {
        Player player = Player.Instance;
        player.AddEnergy(amount);
    }

    public void AddHygiene(float amount)
    {
        Player player = Player.Instance;
        player.AddHygiene(amount);
    }

    public void AddHeat(float amount)
    {
        Player player = Player.Instance;
        player.AddHeat(amount); 
    }

    public float GetPlayerHeightNormal()
    {
        Player player = Player.Instance;
        return player.GetPlayerHeightNormal();
    }

    public float GetPlayerHeightCrouched()
    {
        Player player = Player.Instance;
        return player.GetPlayerHeightCrouched();
    }

    public void SetPlayerHeight(float height, bool stayOnFloorPlane = true)
    {
        Player player = Player.Instance;
        player.SetPlayerHeight(height, stayOnFloorPlane);
    }

    public void SetCameraHeight(float height)
    {
        Player player = Player.Instance;
        player.SetCameraHeight(height);
    }

    public float GetNourishment() 
    {
        Player player = Player.Instance;
        return player.nourishment;
    }

    public float GetHydration()
    {
        Player player = Player.Instance;
        return player.hydration;
    }

    public float GetEnergy()
    {
        Player player = Player.Instance;
        return player.energy;
    }

    public float GetAlertness()
    {
        Player player = Player.Instance;
        return player.alertness;
    }

    public float GetHygiene()
    {
        Player player = Player.Instance;
        return player.hygiene;
    }

    public float GetHeat()
    {
        Player player = Player.Instance;
        return player.heat;
    }

    public float GetHeadache()
    {
        Player player = Player.Instance;
        return player.headache;
    }

    public float GetWet()
    {
        Player player = Player.Instance;
        return player.wet;
    }

    public float GetBrokenLeg()
    {
        Player player = Player.Instance;
        return player.brokenLeg;
    }

    public float GetNumb()
    {
        Player player = Player.Instance;
        return player.numb;
    }

    public float GetSick()
    {
        Player player = Player.Instance;
        return player.sick;
    }

    public bool GetIsLockpicking()
    {
        Player player = Player.Instance;
        return player.isLockpicking;
    }

    public bool GetIsHiding()
    {
        Player player = Player.Instance;
        return player.isHiding;
    }

    public bool GetIsCrouched()
    {
        Player player = Player.Instance;
        return player.isCrouched;
    }
}