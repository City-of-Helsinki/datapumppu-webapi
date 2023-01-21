export default function getVideoPosition() {
    let videoPosition = 0;
    const playerApi = flowplayer("#odPlayer")
    if (playerApi) {
        console.log("playerApi.video.time", playerApi.video.time)
        videoPosition = playerApi.video.time
    }

    return videoPosition
}
