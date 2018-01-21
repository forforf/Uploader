# Uploader

A Windows program for watching a directory for any file changes and upload those changes to an S3 Bucket.

For example, I have it setup to monitor the directory my scanner uses so that whenever I scan a file it's automatically uploaded to S3

## AWS Set up
Create a file in your home directory at $HOME/.aws/credentials. Contents:

```
[Uploader]
aws_access_key_id=AKIAJ3JVB55S5EXAMPLE
aws_secret_access_key=00/EXAMPLESECRET/00abc123
```


## Installation.

Download uploader.zip, uncompress and run setup.exe.
