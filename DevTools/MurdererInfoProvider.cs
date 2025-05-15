using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MurderController;

namespace DevTools
{
    public class MurdererInfoProvider
    {
        MurderController murderController = MurderController.Instance;
        Citizen citizen = new Citizen();

        public Vector3Int GetMurdererLocation()
        {
            if (murderController != null)
            {
                Vector3 playerPosition = murderController.currentMurderer.transform.position;
                return new Vector3Int(Mathf.RoundToInt(playerPosition.x),
                                       Mathf.RoundToInt(playerPosition.y),
                                       Mathf.RoundToInt(playerPosition.z));
            }
            return new Vector3Int(0, 0, 0);
        }
        public void SetMurdererLocation(Vector3 loc)
        {
            murderController.currentMurderer.transform.position = loc;
        }
        public string GetMurdererFullName()
        {
            if (murderController != null)
            {
                string firstName = murderController.currentMurderer.firstName.ToString();
                string lastName = murderController.currentMurderer.surName.ToString();
                string fullName = firstName + " " + lastName;
                return fullName;
            }

            return "murderController is null!";
        }
        public void AddPoisoned(float amount, Human who)
        {
            Player player = Player.Instance;
            murderController.currentMurderer.AddPoisoned(amount, player);
        }
        public void KillMurderer()
        {
            murderController.currentMurderer.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, true, true, 1f);
        }
        public void KOMurderer()
        {
            murderController.currentMurderer.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
        }
        public string GetPassword()
        {
            MurderController murderController = MurderController.Instance;
            Il2CppSystem.Collections.Generic.List<int> digits = murderController.currentMurderer.passcode.digits;
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

            if (murderController.currentMurderer.job.employer != null)
            {
                string employer = murderController.currentMurderer.job.employer.name.ToString();
                string jobname = murderController.currentMurderer.job.name.ToString();
                string salary = murderController.currentMurderer.job.salaryString.ToString();

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
