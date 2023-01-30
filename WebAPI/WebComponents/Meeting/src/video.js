
export default function GetVideoPosition () {
    const playerApi = new window.RadiantMP("odPlayer")

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
    
    return videoPosition;
}