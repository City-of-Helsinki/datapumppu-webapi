
import { useTranslation } from 'react-i18next';
import { useEffect, useState } from 'react'
import SeatRow from './SeatRow'
import { iconStyle, itemOpenStyle, itemStyle } from './styles';

const champerStyle = {
    fontSize: "65%",
    height: "60em",
    pageBreakInside: "avoid",
    backgroundColor: "#dedfe1",
    margin: 0,
    padding: 0
}

export default function Voting(props) {
    const [seats, setSeats] = useState([]);
    const [seatMap, setSeatMap] = useState([]);
    const [showVotes, setShowVotes] = useState(false)
    const { t } = useTranslation();

    const { meetingId, caseNumber, voting } = props

    useEffect(() => {
        const fetchData = async () => {
            const response = await fetch(`#--API_URL--#/seats/${meetingId}/${caseNumber}`)
            if (response.status === 200) {
                const data = await response.json();
                setSeats(data)
            }
        }
        fetchData()
    }, [setSeats])

    useEffect(() => {

        const tempSeatMap = []
        seats.forEach(seat => {

            if (!isNaN(seat.seatId)) {

                let voteType = 0;
                const vote = voting.votes.find(vote => vote.name === seat.person)
                if (vote !== undefined) {
                    voteType = vote.voteType
                }
                let name = seat.person;
                if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
                    name += seat.additionalInfoFI?.length > 0 ? ` (${seat.additionalInfoFI})` : ""
                } else {
                    name += seat.additionalInfoSV?.length > 0 ? ` (${seat.additionalInfoSV})` : ""
                }
                tempSeatMap[Number(seat.seatId)] = {
                    name,
                    voteType
                }
            }
        })

        setSeatMap(tempSeatMap)        
    }, [seats])

    return (
        <>
            <div><h3>{t("Voting")}</h3></div>
            <div>{voting.forTitleFI}: {voting.forCount}</div>
            <div>{voting.againstTitleFI}: {voting.againstCount}</div>
            <div>{t("Empty")}: {voting.emptyCount}</div>
            <div>{t("Absent")}: {voting.absentCount}</div>
            
            <div style={showVotes ? itemOpenStyle : itemStyle}>
            <div onClick={() => setShowVotes(!showVotes)}>
                <span style={iconStyle} className={
                    showVotes
                        ? "glyphicon glyphicon-triangle-top"
                        : "glyphicon glyphicon-triangle-bottom"
                } />
                {t("Show vote details")}
            </div>
            {showVotes &&
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
            }
            </div>
        </>
    )
}
