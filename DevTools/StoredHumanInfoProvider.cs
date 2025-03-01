using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools
{
    public class StoredHumanInfoProvider
    {
        public static Human human;

        public string GetPassword()
        {
            Il2CppSystem.Collections.Generic.List<int> digits = human.passcode.digits;
            System.Text.StringBuilder passcodeBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < digits.Count; i++)
            {
                passcodeBuilder.Append(digits[i].ToString());
            }
            string passcodeString = passcodeBuilder.ToString();
            return passcodeString;
        }

        public string GetStoredFullName()
        {
            if (human != null)
            {
                string firstName = human.firstName.ToString();
                string lastName = human.surName.ToString();
                string fullName = firstName + " " + lastName;
                return fullName;
            }

            return "human is null!";
        }

        public float GetBleeding()
        {
            return human.bleeding;
        }
        public void SetBleeding(float amount)
        {
            human.bleeding = amount;
        }
        public void SetTrespassing(bool trespassing)
        {
            human.isTrespassing = trespassing;
        }
        public string GetJob()
        {
            string noJob = "Citizen is jobless.";

            if (human.job.employer != null)
            {
                string employer = human.job.employer.name.ToString();
                string jobname = human.job.name.ToString();
                string salary = human.job.salaryString.ToString();

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
