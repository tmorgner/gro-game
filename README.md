# Grow Game


## Developer notes

There is a pre-commit hook available to validate that no file larger than 
5MB is committed without marking it as git-lfs file. To activate it, set
the following git-config

    git config core.hooksPath .githooks
    
This requires Git 2.9 to work.
  
