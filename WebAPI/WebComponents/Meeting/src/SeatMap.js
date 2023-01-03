
import { useEffect, useState } from 'react'
import SeatRow from './SeatRow'

const champerStyle = {
    fontSize: "65%",
    height: "60em",
    pageBreakInside: "avoid",
    backgroundColor: "#dedfe1",
    margin: 0,
    padding: 0
}

export default function SeatMap(props) {
    const [seats, setSeats] = useState([]);
    const [seatMap, setSeatMap] = useState([]);

    const { meetingId, caseNumber } = props

    useEffect(() => {
        const fetchData = async () => {
            const response = await fetch(`#--API_URL--#/seats/${meetingId}/${caseNumber}`)
            if (response.status === 200) {
                const data = await response.json();
                console.log("data", data)
                setSeats(data)
            }
        }
        fetchData()
    }, [setSeats])

    useEffect(() => {

        const tempSeatMap = []
        seats.forEach(seat => {
            if (!isNaN(seat.seatId)) {
                tempSeatMap[Number(seat.seatId)] = {
                    name: seat.personFI
                }
            }
        })

        setSeatMap(tempSeatMap)        
    }, [seats])

    return (
        <div style={champerStyle}>
            <SeatRow seats={seatMap}></SeatRow>
            <SeatRow rowNr={0} seats={seatMap}></SeatRow>
            <SeatRow rowNr={1} seats={seatMap}></SeatRow>
            <SeatRow rowNr={2} seats={seatMap}></SeatRow>
            <SeatRow rowNr={3} seats={seatMap}></SeatRow>
            <SeatRow rowNr={4} seats={seatMap}></SeatRow>
            <SeatRow rowNr={5} seats={seatMap}></SeatRow>
            <SeatRow rowNr={6} seats={seatMap}></SeatRow>
            <SeatRow rowNr={7} seats={seatMap}></SeatRow>
            <SeatRow rowNr={8} seats={seatMap}></SeatRow>
            <SeatRow rowNr={10} seats={seatMap} isQuest={true}></SeatRow>
        </div>
    )
}
