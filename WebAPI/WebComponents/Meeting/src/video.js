
/**
 * Gets the current playback position of the RMP (Radiant Media Player) video player.
 * 
 * This function accesses the global RMP player instances and retrieves the current
 * playback time from the first available player instance.
 * 
 * @returns {number} The current video position in seconds. Returns 0 if no player
 *                   instance is available or if the player API is not accessible.
 * 
 * @example
 * // Get current video position
 * const currentTime = GetVideoPosition();
 * console.log(`Video is at ${currentTime} seconds`);
 * 
 */
export default function GetVideoPosition() {
    const instances = window?.rmpGlobals?.rmpInstances
    const playerApi = instances ? Object.values(instances)[0] : null

    if (!playerApi) {
        return 0
    }

    // Get current time in milliseconds
    const videoPosition = playerApi.currentTime

    // Convert milliseconds to seconds
    return videoPosition / 1000;
}