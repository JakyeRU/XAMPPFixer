<p align="center">
    <a href="https://github.com/JakyeRU/Larascord" target="_blank">
        <img src="https://raw.githubusercontent.com/JakyeRU/XAMPPFixer/main/logos/XAMPP Fixer-logos_white.png" height=200>
    </a>
</p>

<p align="center">
    <img src="https://img.shields.io/github/v/release/jakyeru/xamppfixer?color=blue&style=for-the-badge" alt="release">
</p>

# About XAMPP Fixer
XAMPP Fixer is a script created in C# which fixes the common "Error: MySQL shutdown unexpectedly." error from <a href="https://www.apachefriends.org/">XAMPP</a>.

# What does it do?
XAMPP Fixer performs the following actions:
* [OPTIONAL] Creates a backup of existing database files in case of failure.
    * The backup will be created at `\xampp\mysql\data-backups\data-CURRENT_UNIX_TIMESTAMP`
* Renames `\xampp\mysql\data` to `\xampp\mysql\data-temp`.
* Copies `\xampp\mysql\backup` to `\xampp\mysql\data`.
* Moves required database files from `\xampp\mysql\data-temp` back to `\xampp\mysql\data`, excluding a few files, such as `mysql`, `performance_schema`, `phpmyadmin`, and `test`.
* Deletes the temporary directory `\xampp\mysql\data-temp`

# Why not do this manually?
I've done this manually each time XAMPP broke, but after some time it gets repetitive, so I've created this script.