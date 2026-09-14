
import { useEffect, useState } from 'react'
import SeatRow from './SeatRow'

/* 
    SEAT MAP RESPONSIVENESS
    
    min-width of 815px looks arbitrary, but hear me out:

    Each row has 10 seats
    Each seat width is defined to be 8% of the container width (SeatRow.js > seatStyle)

    We want each seat to be about ~65px wide (based on testing) so that the names are readable

    Therefore the container size is: 65px / 8% = 65px / 0.08 = 812px = ~815px

    So the min-width for the container needs to be about 815px,
    because that makes the 8% width seats about 65px wide -> names should display fine
*/
const chamberStyle = {
    fontSize: "65%",
    height: "60em",
    pageBreakInside: "avoid",
    backgroundColor: "#dedfe1",
    minWidth: "815px",
    margin: 0,
    padding: 0
}

export default function SeatMap(props) {
    const [seatMap, setSeatMap] = useState([]);

    const { seats } = props

    useEffect(() => {
        const tempSeatMap = []
        seats.forEach(seat => {
            if (!isNaN(seat.seatId)) {

                let name = seat.person;
                if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
                    name += seat.additionalInfoFI?.length > 0 ? ` (${seat.additionalInfoFI})` : ""
                } else {
                    name += seat.additionalInfoSV?.length > 0 ? ` (${seat.additionalInfoSV})` : ""
                }
                tempSeatMap[Number(seat.seatId)] = { name }
            }
        })

        setSeatMap(tempSeatMap)
    }, [seats])

    return (
        <div style={{ overflowX: "auto" }}>
            <div style={chamberStyle}>
                <SeatRow showName={true} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={0} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={1} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={2} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={3} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={4} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={5} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={6} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={7} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={8} seats={seatMap}></SeatRow>
                <SeatRow showName={true} rowNr={10} seats={seatMap} isQuest={true}></SeatRow>
            </div>
        </div>
    )
}
