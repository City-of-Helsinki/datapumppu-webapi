
import { useTranslation } from 'react-i18next';
import { useEffect, useState } from 'react'
import SeatRow from './SeatRow'
import { jsPDF } from "jspdf"
import { html2canvas } from "html2canvas";
import { agendaButtonStyle, itemOpenStyle, itemStyle, linkStyle } from './styles';
import { FaCaretDown, FaCaretUp } from "react-icons/fa";

const champerStyle = {
    fontSize: "65%",
    height: "50em",
    pageBreakInside: "avoid",
    backgroundColor: "#dedfe1",
    margin: 0,
    padding: 0
}

const voteListContainerStyle = {
    columnCount: 3,
    columnGap: "10px",
    boxSizing: "border-box",
    fontSize: "1em"
}


export default function Voting(props) {
    const [seats, setSeats] = useState([]);
    const [showColors, setShowColors] = useState(true);
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

    const createVoterElement = (vote) => {
        return (
            <div>{ vote.name}</div>
        )
    }

    const downloadPDF = (e) => {
        e.preventDefault()
        let doc = new jsPDF("portrait", 'pt', 'A4');
        doc.setFontSize(8)
        doc.html(document.getElementById('print-area'), {
          html2canvas: {
            scale: 0.7
          },
          callback: () => {
            doc.save('vote.pdf');
          },
          margin: [40, 200, 60, 40]
        })    
    }

    const toggleColors = () => {
        setShowColors(!showColors)
    }

    return (
        <div id="print-area">
            <div><h3>{t("Voting")}</h3></div>
            <div>{voting.forTitleFI}: {voting.forCount}</div>
            <div>{voting.againstTitleFI}: {voting.againstCount}</div>
            <div>{t("Empty")}: {voting.emptyCount}</div>
            <div>{t("Absent")}: {voting.absentCount}</div>
            <div>
            <button style={agendaButtonStyle} onClick={() => setShowVotes(!showVotes)} data-html2canvas-ignore={"true"}>
            <div style={{ paddingRight: "10px", marginTop: "4px" }}>
                        {showVotes
                            ? <FaCaretUp />
                            : <FaCaretDown />}
                    </div>
                {t("Show vote details")}
            </button>
            {showVotes &&
                <div>
                    <div style={champerStyle}>
                        <SeatRow showColors={showColors} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={0} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={1} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={2} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={3} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={4} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={5} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={6} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={7} seats={seatMap}></SeatRow>
                        <SeatRow showColors={showColors} rowNr={8} seats={seatMap}></SeatRow>
                    </div>
                    <button onClick={toggleColors} data-html2canvas-ignore={"true"} style={agendaButtonStyle}>
                        {showColors ? t("Show black and white vote map") : t("Show vote map with colors")}
                    </button>
                    <button onClick={downloadPDF} data-html2canvas-ignore={"true"} style={agendaButtonStyle}>{t("Download voting map pdf")}</button>
                    <div style={voteListContainerStyle} data-html2canvas-ignore={"true"}>
                        <div>{t("FOR")}</div>
                        { voting && seatMap.filter(vote => vote.voteType === 0).map(vote => createVoterElement(vote)) }

                        <div>{t("AGAINST")}</div>
                        { voting && seatMap.filter(vote => vote.voteType === 1).map(vote => createVoterElement(vote)) }

                        <div>{t("EMPTY")}</div>
                        { voting && seatMap.filter(vote => vote.voteType === 2).map(vote => createVoterElement(vote)) }

                        <div>{t("ABSENT")}</div>
                        { voting && seatMap.filter(vote => vote.voteType === 3).map(vote => createVoterElement(vote)) }

                    </div>
                </div>
            }
            </div>
        </div>
    )
}
