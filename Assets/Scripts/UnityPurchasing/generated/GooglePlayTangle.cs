// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("w0BOQXHDQEtDw0BAQd5x5JeBwJQRIBb/BBxjSdiilgQi02kdodSqxgzVg+8/bu73mDX+NujrJCGvhJb4hSbWQ81XeCfsA0+e46fUnh3unbifnAZREhFRbCvdPgfCu4QT6eCpp5Lgjukru/J94wQSfPdLtOV6NUDg+OKjLdSsKZVbLmgAn7LGQVqd0Ee/r9Esxmepk67k6k9ubZHYtBIoi56ozdEHO2OMWbXel161k1fsAfkYccNAY3FMR0hrxwnHtkxAQEBEQUJXVwv0fdJdtbYRJOTCm7Bapimfsm8SCUIiLZubKiy5gM7btMWAcKwSVMPFXpRGokhw0EGbNsjwVVnGK3WHp9o3supBOedgsa/xlVUN3NdsvyD4uQSQIRnlyENCQEFA");
        private static int[] order = new int[] { 1,13,7,5,10,9,12,8,11,13,12,12,13,13,14 };
        private static int key = 65;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
