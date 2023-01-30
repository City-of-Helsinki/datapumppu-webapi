
export default function GetVideoPosition () {
    const playerApi = window._icareus["playerObject"]

    if (!playerApi) {
        return 0
    }

    const videoPosition = playerApi.getCurrentTime()
    console.log("getCurrentTime", videoPosition)
    console.log("getReady", playerApi.getReady())
    console.log("getPlayerVersion", playerApi.getPlayerVersion())
    console.log("getStreamType", playerApi.getStreamType())
    console.log("getControls", playerApi.getControls())
    console.log("_icareus", _icareus)
    console.log("window", window)
    
    return videoPosition;
}