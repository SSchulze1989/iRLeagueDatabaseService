using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Entities.Members
{
    public class LeagueUserEntity : MappableEntity, IClientUser
    {
        [Key]
        public long AdminId { get; set; }

        public string UserName { get; set; }

        public override object MappingId => AdminId;

        public byte[] PwSalt { get; set; }

        public byte[] PwHash { get; set; }

        [ForeignKey(nameof(Member))]
        public long? MemberId { get; set; }
        public virtual LeagueMemberEntity Member { get; set; }

        public AdminRights AdminRights { get; set; }

        public LeagueUserEntity() { }

        public bool CheckCredentials(byte[] password)
        {
            if (password == null)
                return false;

            var salt = PwSalt;
            var hash = CalculateHash(salt, password);

            if (hash.SequenceEqual(PwHash))
                return true;
            return false;
        }

        private byte[] CalculateHash(byte[] salt, byte[] value)
        {
            byte[] SaltedValue = new byte[salt.Length + value.Length];
            System.Buffer.BlockCopy(salt, 0, SaltedValue, 0, salt.Length);
            System.Buffer.BlockCopy(value, 0, SaltedValue, salt.Length, value.Length);

            System.Security.Cryptography.HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed();
            return algorithm.ComputeHash(SaltedValue);
        }

        public bool Authorize(AdminRights rights)
        {
            return AdminRights.HasFlag(rights);
        }

        public bool SetPassword(byte[] oldPassword, byte[] newPassword)
        {
            byte[] salt = new byte[32];
            System.Security.Cryptography.RNGCryptoServiceProvider.Create().GetBytes(salt);

            if (PwSalt == null && PwHash == null) {
                PwSalt = new byte[salt.Length];
                System.Buffer.BlockCopy(salt, 0, PwSalt, 0, salt.Length);
                var hashBuffer = CalculateHash(salt, newPassword);
                PwHash = new byte[hashBuffer.Length];
                System.Buffer.BlockCopy(hashBuffer, 0, PwHash, 0, hashBuffer.Length);
                return true;
            }
            else if (CheckCredentials(oldPassword))
            {
                System.Buffer.BlockCopy(salt, 0, PwSalt, 0, salt.Length);
                var hashBuffer = CalculateHash(salt, newPassword);
                System.Buffer.BlockCopy(hashBuffer, 0, PwHash, 0, hashBuffer.Length);
                return true;
            }
            return false;
        }
    }
}
