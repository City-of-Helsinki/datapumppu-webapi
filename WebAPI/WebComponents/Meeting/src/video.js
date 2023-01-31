
export default function GetVideoPosition () {

    const playerApi = window._icareus.playerObject
    if (!playerApi) {
        return 0
    }

    const videoPosition = playerApi.getCurrentTime()
    return videoPosition / 1000;
}