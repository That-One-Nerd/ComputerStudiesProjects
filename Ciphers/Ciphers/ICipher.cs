namespace CipherCrypt;

public interface ICipher
{
    public static abstract string Name { get; }

    public static abstract void Cipher(bool encrypt, string[] args);
}
