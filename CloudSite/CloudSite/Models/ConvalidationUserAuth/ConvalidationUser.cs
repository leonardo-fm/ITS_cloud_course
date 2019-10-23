using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using CloudSite.Model;

namespace CloudSite.Models.ConvalidationUserAuth
{
    public class ConvalidationUser
    {
        private string _userName;
        private string _userEmail;
        private string _userPassword;

        public ConvalidationUser(User user)
        {
            _userName = user.userName;
            _userEmail = user.userEmail;
            _userPassword = user.userPassword;
        }

        public bool isTheUserHaveValidParametres()
        {
            if (isTheUserHaveValidUsername() &&
                isTheUserHaveValidEmail() &&
                isTheUserHaveValidPassword())
                return true;
            else
                return false;
        }

        public bool isTheUserHaveValidUsername()
        {
            string regUsername = @"^(?=[A-Za-z0-9])(?!.*[._()\[\]-]{2})[A-Za-z0-9._()\[\]-]{3,20}$";
            return Regex.IsMatch(_userName, regUsername);
        }

        public bool isTheUserHaveValidPassword()
        {
            string regPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
            return Regex.IsMatch(_userPassword, regPassword);
        }

        public bool isTheUserHaveValidEmail()
        {
            EmailAddressAttribute emailChecker = new EmailAddressAttribute();
            return emailChecker.IsValid(_userEmail);
        }

        public string cryptUserPassword(string userPassword)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(userPassword);
            data = new SHA256Managed().ComputeHash(data);
            string hash = System.Text.Encoding.ASCII.GetString(data);
            
            return hash;
        }

        public bool checkPasswordIsTheSame(string passwordToCheck, string passwordDB)
        {
            return cryptUserPassword(passwordToCheck) == passwordDB;
        }
    }
}