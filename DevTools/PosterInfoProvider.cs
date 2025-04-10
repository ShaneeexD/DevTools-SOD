﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools
{
    public class PosterInfoProvider
    {
        public string GetPassword()
        {
            CasePanelController casePanelController = CasePanelController.Instance;
            Il2CppSystem.Collections.Generic.List<int> digits = casePanelController.activeCase.job.poster.passcode.digits;
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

            if (CasePanelController.Instance.activeCase.job.poster.job.employer != null)
            {
                string employer = CasePanelController.Instance.activeCase.job.poster.job.employer.name.ToString();
                string jobname = CasePanelController.Instance.activeCase.job.poster.job.name.ToString();
                string salary = CasePanelController.Instance.activeCase.job.poster.job.salaryString.ToString();

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
