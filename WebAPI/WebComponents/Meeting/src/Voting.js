
import { useTranslation } from 'react-i18next';
import { useEffect, useState } from 'react'
import SeatRow from './SeatRow'
import { jsPDF } from "jspdf"
import { html2canvas } from "html2canvas";
import { iconStyle, itemOpenStyle, itemStyle } from './styles';

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

    const togleColors = () => {
        setShowColors(!showColors)
    }

    return (
        <div id="print-area">
            <div><h3>{t("Voting")}</h3></div>
            <div>{voting.forTitleFI}: {voting.forCount}</div>
            <div>{voting.againstTitleFI}: {voting.againstCount}</div>
            <div>{t("Empty")}: {voting.emptyCount}</div>
            <div>{t("Absent")}: {voting.absentCount}</div>
            
            <div style={showVotes ? itemOpenStyle : itemStyle}>
            <div onClick={() => setShowVotes(!showVotes)} data-html2canvas-ignore={"true"}>
                <span style={iconStyle} className={
                    showVotes
                        ? "glyphicon glyphicon-triangle-top"
                        : "glyphicon glyphicon-triangle-bottom"
                } />
                {t("Show vote details")}
            </div>
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
                    <div onClick={togleColors} data-html2canvas-ignore={"true"}>
                        {showColors ? t("Show black and white vote map") : t("Show vote map with colors")}
                    </div>
                    <div onClick={downloadPDF} data-html2canvas-ignore={"true"}>{t("Download voting map pdf")}</div>
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
