// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("PphQ5OoKDVn9ddycbm6Bpio+8P1I+4XRQbEzjKPZqm96tpxIwYfebW/TnP+c6hQL4/snQLaAvJAbZ9CCIYgh3781abkLABN1WmSOiXsvF4LkDTkVOWEQqCk4Yz8hKbi7VtlnCKDCxIBPX1fcxJBgzTeXKWLH1yG9YTx89RCmdVsDI07bRwKTOOSIIHhp1eKOMza8+GpI3DIXmb461kjkycxYXnEtGNVkjeHnbz40ZC+WhoLhxkVLRHTGRU5GxkVFRPkuK5GxMxTuSCaTG85F/2q5s/ryhVoAdzssf1dQkpWOKYfSbBroZrVm4+tQ9hkNP3ZgSdMRjG6fl8bxBE4q/y2tp1Z0xkVmdElCTW7CDMKzSUVFRUFERyPO/cdvwXupKUZHRURF");
        private static int[] order = new int[] { 5,3,11,13,11,6,10,7,9,13,13,11,12,13,14 };
        private static int key = 68;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
