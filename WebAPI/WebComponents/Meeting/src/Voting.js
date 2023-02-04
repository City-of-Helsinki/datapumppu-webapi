
import { useTranslation } from 'react-i18next';
import { useEffect, useState } from 'react'
import SeatRow from './SeatRow'
import { jsPDF } from "jspdf"
import html2canvas from "html2canvas" // DO NOT REMOVE THIS
import { agendaButtonStyle, titleStyle, linkStyle } from './styles';
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
    fontSize: "90%"
}


export default function Voting(props) {
    const [seats, setSeats] = useState([]);
    const [showColors, setShowColors] = useState(true);
    const [seatMap, setSeatMap] = useState([]);
    const [showVotes, setShowVotes] = useState(false)
    const [showHeader, setShowHeader] = useState(false)
    const { t } = useTranslation();

    const { meetingId, caseNumber, voting, index, title, updated, updatedCaseNumber } = props

    useEffect(() => {
        if (updatedCaseNumber == caseNumber) {
            fetchData()
        }
    }, [updated, updatedCaseNumber])

    useEffect(() => {
        fetchData()
    }, [])

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

    const fetchData = async () => {
        const response = await fetch(`#--API_URL--#/seats/${meetingId}/${caseNumber}`)
        if (response.status === 200) {
            const data = await response.json();
            setSeats(data)
        }
    }

    const createVoterElement = (vote) => {
        return (
            <div>{vote.name}</div>
        )
    }

    const downloadPDF = (e) => {

        setShowHeader(true)
        setTimeout(() => {
            e.preventDefault()
            let doc = new jsPDF("landscape", 'pt', 'A4');
            doc.setFontSize(8)
            doc.html(document.getElementById(`print-area-${index}`), {
                html2canvas: {
                    scale: 0.7
                },
                callback: () => {
                    doc.save('vote.pdf');
                },
                margin: [10, 10, 10, 10]
            })
            setShowHeader(false)
        })
    }

    const toggleColors = () => {
        setShowColors(!showColors)
    }

    const getForTitle = (voting) => {
        if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
            return `${t('FOR')}: ${voting.forTitleFI}`
        } else {
            return `${t('FOR')}: ${voting.forTitleSV}`
        }
    }

    const getForText = (voting) => {
        if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
            return `${voting.forTextFI}`
        } else {
            return `${voting.forTextSV}`
        }
    }

    const getAgainstTitle = (voting) => {
        if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
            return `${t('AGAINST')}: ${voting.againstTitleFI}`
        } else {
            return `${t('AGAINST')}: ${voting.againstTitleSV}`
        }
    }

    const getAgainstText = (voting) => {
        if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
            return `${voting.againstTextFI}`
        } else {
            return `${voting.againstTextSV}`
        }
    }

    return (
        <div id={`print-area-${index}`}>
            {showHeader && (<h3>{caseNumber}. {title}</h3>)}
            <h4>{t("Voting")}</h4>
            <div>{getForTitle(voting)}: {voting.forCount}</div>
            {getForText(voting).length > 0 && <div style={{marginLeft: "1rem", fontStyle: "italic"}}>{getForText(voting)}</div>}
            <div>{getAgainstTitle(voting)}: {voting.againstCount}</div>
            {getAgainstText(voting).length > 0 && <div style={{marginLeft: "1rem", fontStyle: "italic"}}>{getAgainstText(voting)}</div>}
            <div>{t("Empty")}: {voting.emptyCount}</div>
            <div>{t("Absent")}: {voting.absentCount}</div>
            <div>
                <div style={{ padding: "30px 10px 0 0" }}>
                    <button style={agendaButtonStyle} onClick={() => setShowVotes(!showVotes)} data-html2canvas-ignore={"true"}>
                        <div style={{ paddingRight: "10px", marginTop: "4px" }}>
                            {showVotes
                                ? <FaCaretUp />
                                : <FaCaretDown />}
                        </div>
                        {t("Show vote details")}
                    </button>
                </div>
                {showVotes &&
                    <div style={{ display: 'flex', flexDirection: 'column' }}>
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
                        <a href='javascript:void(0)' onClick={toggleColors} data-html2canvas-ignore={"true"} style={linkStyle}>
                            {showColors ? t("Show black and white vote map") : t("Show vote map with colors")}
                        </a>
                        <a href="javascript:void(0)" onClick={downloadPDF} data-html2canvas-ignore={"true"} style={linkStyle}>
                            {t("Download voting map pdf")}
                        </a>
                        <div style={voteListContainerStyle} data-html2canvas-ignore={"true"}>
                            <p>
                                <div>{t("FOR")}</div>
                            </p>
                            <p>
                                {voting && seatMap.filter(vote => vote.voteType === 0).map(vote => createVoterElement(vote))}
                            </p>
                            <p>
                                <div>{t("AGAINST")}</div>
                            </p>
                            <p>
                                {voting && seatMap.filter(vote => vote.voteType === 1).map(vote => createVoterElement(vote))}
                            </p>
                            <p>
                                <div>{t("EMPTY")}</div>
                            </p>
                            <p>
                                {voting && seatMap.filter(vote => vote.voteType === 2).map(vote => createVoterElement(vote))}
                            </p>
                            <p>
                                <div>{t("ABSENT")}</div>
                            </p>
                            <p>
                                {voting && seatMap.filter(vote => vote.voteType === 3).map(vote => createVoterElement(vote))}
                            </p>
                        </div>
                    </div>
                }
            </div>
        </div>
    )
}
