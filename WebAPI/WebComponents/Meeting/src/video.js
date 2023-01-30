import flowplayer from '@flowplayer/player'

export default function GetVideoPosition () {
    const playerApi = new window.RadiantMP("#odPlayer")
    if (playerApi) {
        console.log("playerApi.video.time", playerApi.getCurrentTime())
        videoPosition = playerApi.getCurrentTime()
    }
}