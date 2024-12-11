using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

namespace System
{
    public class CustomCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // 항상 true를 반환하여 모든 인증서를 신뢰함
            Debug.Log("ValidateCertificate");
            
            // X509Certificate2 cert = new X509Certificate2(certificateData);
            // string expectedThumbprint = "YOUR_CERTIFICATE_THUMBPRINT";
            //
            // // 인증서의 thumbprint을 검증
            // return cert.Thumbprint == expectedThumbprint;
            
            return true;
        }
    }
}

