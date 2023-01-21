import flowplayer from '@flowplayer/player'

export default function GetVideoPosition () {
    const playerApi = flowplayer("#odPlayer")
    if (playerApi) {
        console.log("playerApi.video.time", playerApi.video.time)
        videoPosition = playerApi.video.time
    }
}