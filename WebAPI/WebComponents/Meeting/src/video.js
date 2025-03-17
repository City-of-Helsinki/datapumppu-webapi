
export default function GetVideoPosition () {

    const playerApi = window?.rmpGlobals?.rmpInstances?.[0]
    if (!playerApi) {
        return 0
    }

    const videoPosition = playerApi.getCurrentTime()
    return videoPosition / 1000;
}