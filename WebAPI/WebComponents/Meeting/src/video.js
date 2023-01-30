
export default function GetVideoPosition () {

    console.log("window._icareus", window._icareus)
    console.log("window._icareus", window._icareus.playerObject)
    console.log("window._icareus", window._icareus["playerObject"])
    const playerApi = window._icareus.playerObject

    if (!playerApi) {
        return 0
    }

    const videoPosition = playerApi.getCurrentTime()
    console.log("getCurrentTime", videoPosition)
    console.log("getReady", playerApi.getReady())
    console.log("getPlayerVersion", playerApi.getPlayerVersion())
    console.log("getStreamType", playerApi.getStreamType())
    console.log("getControls", playerApi.getControls())
    
    return videoPosition / 1000;
}