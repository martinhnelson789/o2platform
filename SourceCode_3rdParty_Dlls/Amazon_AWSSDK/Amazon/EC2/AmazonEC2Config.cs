﻿namespace Amazon.EC2
{
    using System;

    public class AmazonEC2Config
    {
        private bool fUseSecureString = true;
        private int maxErrorRetry = 3;
        private string proxyHost;
        private string proxyPassword;
        private int proxyPort = -1;
        private string proxyUsername;
        private string serviceURL = "https://ec2.amazonaws.com";
        private string serviceVersion = "2009-11-30";
        private string signatureMethod = "HmacSHA256";
        private string signatureVersion = "2";
        private string userAgent = "AWS SDK for .NET/1.0.9";

        public bool IsSetMaxErrorRetry()
        {
            return (this.maxErrorRetry >= 0);
        }

        public bool IsSetProxyHost()
        {
            return (this.proxyHost != null);
        }

        internal bool IsSetProxyPassword()
        {
            return !string.IsNullOrEmpty(this.proxyPassword);
        }

        public bool IsSetProxyPort()
        {
            return (this.proxyPort >= 0);
        }

        internal bool IsSetProxyUsername()
        {
            return !string.IsNullOrEmpty(this.proxyUsername);
        }

        public bool IsSetServiceURL()
        {
            return (this.serviceURL != null);
        }

        public bool IsSetSignatureMethod()
        {
            return (this.signatureMethod != null);
        }

        public bool IsSetSignatureVersion()
        {
            return (this.signatureVersion != null);
        }

        public bool IsSetUserAgent()
        {
            return (this.userAgent != null);
        }

        public AmazonEC2Config WithMaxErrorRetry(int maxErrorRetry)
        {
            this.maxErrorRetry = maxErrorRetry;
            return this;
        }

        public AmazonEC2Config WithProxyHost(string proxyHost)
        {
            this.proxyHost = proxyHost;
            return this;
        }

        public AmazonEC2Config WithProxyPassword(string password)
        {
            this.proxyPassword = password;
            return this;
        }

        public AmazonEC2Config WithProxyPort(int proxyPort)
        {
            this.proxyPort = proxyPort;
            return this;
        }

        public AmazonEC2Config WithProxyUsername(string userName)
        {
            this.proxyUsername = userName;
            return this;
        }

        public AmazonEC2Config WithServiceURL(string serviceURL)
        {
            this.serviceURL = serviceURL;
            return this;
        }

        public AmazonEC2Config WithSignatureMethod(string signatureMethod)
        {
            this.signatureMethod = signatureMethod;
            return this;
        }

        public AmazonEC2Config WithSignatureVersion(string signatureVersion)
        {
            this.signatureVersion = signatureVersion;
            return this;
        }

        public AmazonEC2Config WithUserAgent(string userAgent)
        {
            this.userAgent = userAgent;
            return this;
        }

        public AmazonEC2Config WithUseSecureStringForAwsSecretKey(bool fSecure)
        {
            this.fUseSecureString = fSecure;
            return this;
        }

        public int MaxErrorRetry
        {
            get
            {
                return this.maxErrorRetry;
            }
            set
            {
                this.maxErrorRetry = value;
            }
        }

        public string ProxyHost
        {
            get
            {
                return this.proxyHost;
            }
            set
            {
                this.proxyHost = value;
            }
        }

        public string ProxyPassword
        {
            get
            {
                return this.proxyPassword;
            }
            set
            {
                this.proxyPassword = value;
            }
        }

        public int ProxyPort
        {
            get
            {
                return this.proxyPort;
            }
            set
            {
                this.proxyPort = value;
            }
        }

        public string ProxyUsername
        {
            get
            {
                return this.proxyUsername;
            }
            set
            {
                this.proxyUsername = value;
            }
        }

        public string ServiceURL
        {
            get
            {
                return this.serviceURL;
            }
            set
            {
                this.serviceURL = value;
            }
        }

        public string ServiceVersion
        {
            get
            {
                return this.serviceVersion;
            }
        }

        public string SignatureMethod
        {
            get
            {
                return this.signatureMethod;
            }
            set
            {
                this.signatureMethod = value;
            }
        }

        public string SignatureVersion
        {
            get
            {
                return this.signatureVersion;
            }
            set
            {
                this.signatureVersion = value;
            }
        }

        public string UserAgent
        {
            get
            {
                return this.userAgent;
            }
            set
            {
                this.userAgent = value;
            }
        }

        public bool UseSecureStringForAwsSecretKey
        {
            get
            {
                return this.fUseSecureString;
            }
            set
            {
                this.fUseSecureString = value;
            }
        }
    }
}

