# vision-framework
a c-sharp based online collaborative writing system

# deploy
## on local machine

1. clone the github repo
2. create a database in visual studio's `sql server object explorer`: `sql server/(localdb)/mssqllocaldb/vision-dev`
3. import the sql database from `./db-dev/` or run the sql query `db-create-table.sql`
4. change the connection string in `appsettings.json` if you change the name of the database
5. open project vision

# snapshots

* `snapshot for v0.6.0`
  ![v0.6.0](/github/images/vision-0.6.0-1.png)

# release notes

## 0.7.0 (with VML v1.2.0)

* `vml` annonymous variables
* `vml` change in property-variable syntax from `@{[var] #func|params, ...}` to `@{[var] #func:params, ...}`
* `vml` change selective branch name from `ifbranch` to `if`
* `new` `editor` syntax highlignting
* `new` `editor` template model auto-completion and suggestions
* `new` table template
* `new` category and navigation support
* `change` remove side bar from some of the system pages
* `change` auto-load system templates
* `fix` the problem of wrong user account status
* `fix` display unexpectancy in tables

## 0.6.0 (with VML v1.1.47)

* `new` side bars and history rendering (not complete)
* `new` add basic user support, and by default displays the user's ip address
* `new` basic set of compulsory models and templates
* `new` add support for code snippets
* `change` changed ifbranch condition
* `change` changed basic set of default system variables
* `fix` fix 17 issues concerning text data evaluation
* `fix` the memory of outer-cascade variable
* `fix` evaluate text data variable, while output other types as raw.

## 0.5.1 (with VML v1.1.25)
