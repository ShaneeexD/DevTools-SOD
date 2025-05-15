using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DevTools
{
    public class VictimInfoHelper
    {
        MurderController murderController = MurderController.Instance;
        public Vector3Int GetVictimLocation()
        {
            if (murderController != null)
            {
                Vector3 playerPosition = murderController.currentVictim.transform.position;
                return new Vector3Int(Mathf.RoundToInt(playerPosition.x),
                                       Mathf.RoundToInt(playerPosition.y),
                                       Mathf.RoundToInt(playerPosition.z));
            }
            return new Vector3Int(0, 0, 0);
        }
        public void SetVictimLocation(Vector3 loc)
        {
            murderController.currentVictim.transform.position = loc;
        }
        public string GetVictimFullName()
        {
            if (murderController != null)
            {
                string firstName = murderController.currentVictim.firstName.ToString();
                string lastName = murderController.currentVictim.surName.ToString();
                string fullName = firstName + " " + lastName;
                return fullName;
            }

            return "murderController is null!";
        }
        public void AddPoisoned(float amount, Human who)
        {
            Player player = Player.Instance;
            murderController.currentVictim.AddPoisoned(amount, player);
        }
        public void KillVictim()
        {
            murderController.currentVictim.RecieveDamage(99999f, murderController.currentMurderer, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, true, true, 1f);
        }
        public void KOVictim()
        {
            murderController.currentVictim.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
        }
        public string GetPassword()
        {
            MurderController murderController = MurderController.Instance;
            Il2CppSystem.Collections.Generic.List<int> digits = murderController.currentVictim.passcode.digits;
            System.Text.StringBuilder passcodeBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < digits.Count; i++)
            {
                passcodeBuilder.Append(digits[i].ToString());
            }
            string passcodeString = passcodeBuilder.ToString();
            return passcodeString;
        }
        public string GetJob()
        {
            string noJob = "Citizen is jobless.";

            if (murderController.currentVictim.job.employer != null)
            {
                string employer = murderController.currentVictim.job.employer.name.ToString();
                string jobname = murderController.currentVictim.job.name.ToString();
                string salary = murderController.currentVictim.job.salaryString.ToString();

                string jobDec = "Employer: " + employer + Environment.NewLine + "Job: " + jobname + Environment.NewLine + "Salary: " + salary;
                
                return jobDec;
            }
            else
            {
                return noJob;
            }
        }
    }
}
