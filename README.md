An implementation of a secure ASP.NET server that can be used by users to delegate the process of signature creation and verification. 

### Usage:
- Signature creation
    - If you want to create a signature, you need to register an account. For security, Two Factor Authentication, alongside a strong password, is required in order to create signatures of multiple documents. 
    - Upload your files, click on "Sign" and receive your signature in the form of a ZIP archive, that contains your files and a signature that is tied to your account.
- Verification
    - In order to verify a signature, an account is not needed. You upload the signature ZIP archive, and if the signature has not been tampered with, you will receive the signer's identity as a result.