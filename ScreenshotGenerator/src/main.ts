import { app, BrowserWindow, ipcRenderer } from "electron";
import * as path from "path";
import * as url from "url";

let mainWindow: Electron.BrowserWindow;

const electron = require('electron');
const dialog = electron.dialog;
const { ipcMain } = require('electron');
exports.selectDirectory = function () {
  dialog.showOpenDialog(mainWindow, {
    properties: ['openDirectory']
  });
}

interface ICommandLineArguments {
  key: string;
  value?: string;
}

function parseCommandLineArguments(): ICommandLineArguments[] {
  let args: ICommandLineArguments[] = new Array<ICommandLineArguments>();
  const commandLineArgs = process.argv;
  args.push({ key: 'executable', value: commandLineArgs[0] });
  args.push({ key: 'script', value: commandLineArgs[1] });
  for (let a = 2; a < commandLineArgs.length; ++a) {
    let arg = commandLineArgs[a];

    if (arg.indexOf('=') !== -1) {
      let argument = arg.split('=');

      let entry: ICommandLineArguments = {
        key: argument[0],
        value: argument[1]
      }

      args.push(entry);
    }
    else {
      let entry: ICommandLineArguments = {
        key: arg
      }
      args.push(entry);
    }
  }

  return args;
}

function createWindow() {
  let width = 800;
  let height = 800;
  let gltf: string = "";
  let headless = false;
  let manifest = "";

  const commandLineArgs = parseCommandLineArguments();

  for (let i = 0; i < commandLineArgs.length; ++i) {
    const commandLineArg = commandLineArgs[i];
    switch (commandLineArg.key) {
      case 'width': {
        width = parseInt(commandLineArg.value);
        break;
      }
      case 'height': {
        height = parseInt(commandLineArg.value);
        break;
      }
      case 'gltf': {
        gltf = commandLineArg.value;
        break;
      }
      case 'manifest': {
        manifest = commandLineArg.value;
        break;
      }
      case 'headless': {
        console.log(commandLineArg);
        headless = true;
        break;
      }
      case 'executable': {
        break;
      }
      case 'script': {
        break;
      }
      default: {
        console.warn('Unrecognized command line argument: ' + commandLineArg.key);
      }
    }
  }

  // Create the browser window.
  mainWindow = new BrowserWindow({
    height: height,
    width: width,
    useContentSize: true,
    show: !headless
  });
 
  mainWindow.setContentSize(width, height);
  const contentSize = mainWindow.getContentSize();

  // Check if the content size has extra pixels and re-adjust size
  mainWindow.setContentSize(width - (contentSize[0] - width), height - (contentSize[1] - height));
  console.log(mainWindow.getContentSize());
  ipcMain.on('synchronous-message', (event: any, arg: string) => {
    if (arg === 'headless') {
      event.returnValue = headless;
    }
    else if (arg === 'gltf') {
      console.log('gltf called: ' + gltf);
      event.returnValue = gltf;
    }
    else if (arg === 'manifest') {
      event.returnValue = manifest;
    }
    else {
      console.log(arg);
    }
  });


  // and load the index.html of the app.
  mainWindow.loadURL(url.format({
    pathname: path.join(__dirname, "../index.html"),
    protocol: "file:",
    slashes: true,
  }));

  // Open the DevTools.
 // mainWindow.webContents.openDevTools();

  // Emitted when the window is closed.
  mainWindow.on("closed", () => {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    mainWindow = null;
  });
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on("ready", createWindow);

// Quit when all windows are closed.
app.on("window-all-closed", () => {
  // On OS X it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", () => {
  // On OS X it"s common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (!mainWindow) {
    createWindow();
  }
});

// In this file you can include the rest of your app"s specific main process
// code. You can also put them in separate files and require them here.