# hadco

## Getting Started

### Prerequisites:

This project requires that you have NodeJS and NPM installed on your computer.
If you don't have them installed (e.g. typing `node -v` or `npm -v` in the
cmd/powershell/terminal results in an error; if they come up with a version
number, they are installed), go to [nodejs.org](https://nodejs.org/en/)
and install the LTS or Current versions of Node, ideally >= 13.3.0. This should
install Node as well as NPM. Once those are installed, you may have to close 
all terminals or programs (like VS Code) that will be using it and open them
again to ensure that they can find the executables in the PATH.

SQL Server 2017+ or Azure SQL are required to run the Occurrences and Employee Roles CSV SQL queries.

### Setup steps

1. First, import the `bacpac` found at `\\dcs-svr3\temp\HadcoBacpacFiles\`.
   Make sure that the imported/created database is called
   `Hadco.Data.HadcoContext` (if you're unfamiliar, [here are instructions](https://docs.microsoft.com/en-us/sql/relational-databases/data-tier-applications/import-a-bacpac-file-to-create-a-new-user-database?view=sql-server-2017#using-the-import-data-tier-application-wizard)
   on how to do this)
1. Start the API by debugging or build/run without debugging, ensuring that
   the `Hadco.Web` project is set as the startup project
1. In the terminal of your choice, cd into the `Hadco.Web` folder inside of 
   this project and then run the command `npm install` and wait for it to
   complete
    - This should also be done after you've pulled the changes and are either
      certain or uncertain about if the changes include front-end packages;
      this may mean that you _always_ run it, and it's not a bad choice
1. Once it's done, you should be able to run the front-end by running
   `npm start`; it should start a webpack dev server and open a browser
    - This allows for immediate compilation of the TypeScript and SASS in 
      the project as well as will refresh the browser once the compilation
      is complete
    - If you're only working on the back-end and don't care about if the
      web app is working, you don't need to do this step

Though it's not a hard-and-fast rule, because of the automatic inclusion of 
assets into the solution in Hadco if you add a `New Item` in the Solution 
Explorer as well as the relative difficulty in seeing the front-end assets 
since they're not included, it's not recommended you use Visual Studio to 
modify front-end files. Instead, using other editors such as [Visual Studio 
Code](https://code.visualstudio.com/) is recommended, making sure to open the
editor in the project directory or the `Hadco.Web` directory.

The front-end assets are found in the `src` directory inside of `Hadco.Web`.

## Migrating data from ComputerEase

Note: This isn't required to get the project up and running.

First you have to have the ComputerEase driver installed:

\\\DCS-SVR3\Hadco\const\odbc\setup.exe

Then just run the Hadco.Integration.ComputerEase project found in the solution. It can take between 10 and 20 minutes to process due to the number of Categories in the system. If the ComputerEase data is on the local server this process takes between 1 and 2 minutes, but over the LAN it takes a bit longer.

## Front-end Asset Inclusion
Previously, the `Hadco.Web.csproj` file explicitly included files in the `Hadco.Web\app` folder, which caused the file to be long and require any new files to be explicitly added, particularly if you didn't use Visual Studio itself to add or delete files.  On top of that, Azure would throw errors, causing the build to fail.

With an update, the `Hadco.Web.csproj` file has been modified to automatically include content from the `dist` folder (which holds the built front-end files).  The line looks like the following:
```xml
<Content Include="dist\**" />
```

If you are adding files (particularly `.ts` or `.scss` files) and they are not included, don't worry!  They're not to be included anyway, since anything that is `Include`d should only be what is needed on production or QA servers.  If you are wanting to exclude certain files or extensions, feel free to add them to an `Exclude` attribute (like `Exclude="*.ts; *.scss"`) to the `Content` tag found in the `Hadco.Web.csproj` file, making sure to separate files or match paths with semicolons. If you want to see what files will be included when the project is built, run `npm run build` inside the `Hadco.Web` directory, and files will be placed in the `dist` folder as they will appear on those environments.
