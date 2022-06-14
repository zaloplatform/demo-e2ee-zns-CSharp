using libsignal;
using libsignal.ecc;
using libsignal.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestE2EEZNS
{
    public class E2EEHelper
    {
        public static IdentityKeyPair generateIdentityKeyPair()
        {
            ECKeyPair identityKeyPairKeys = Curve.generateKeyPair();
            return new IdentityKeyPair(new IdentityKey(identityKeyPairKeys.getPublicKey()), identityKeyPairKeys.getPrivateKey());
        }

        public static uint generateRegistrationId()
        {
            return KeyHelper.generateRegistrationId(false);
        }
    }

}

