function parseParameters() {
    var result = {};
    var parameters = location.href.split("?")[1];
    if (parameters) {
        parameters = parameters.split("&");
        for (var i = 0; i < parameters.length; i++) {
            var parameter = parameters[i].split("=");
            switch (parameter[0]) {
                case "rootUrl": {
                    result.rootUrl = decodeURIComponent(parameter[1]);
                    break;
                }
                case "fileName": {
                    result.fileName = decodeURIComponent(parameter[1]);
                    break;
                }
            }
        }
    }

    return result;
}

function createScene(engine, onSuccess) {
    var parameters = parseParameters();

    if (!parameters.rootUrl && document.referrer) {
        parameters.rootUrl = document.referrer.substr(0, document.referrer.lastIndexOf("/") + 1);
    }

    BABYLON.SceneLoader.Load(parameters.rootUrl, parameters.fileName, engine, function (scene) {
        scene.createDefaultCameraOrLight(true, true, true);
        scene.activeCamera.alpha += Math.PI;

        var hdrTexture = BABYLON.CubeTexture.CreateFromPrefilteredData("environment.dds", scene);
        scene.createDefaultSkybox(hdrTexture, true, (scene.activeCamera.maxZ - scene.activeCamera.minZ) / 2, 0.3);

        onSuccess(scene);
    }, null, function (scene, message) {
        alert(message);
    });
}
