
import { useTranslation } from 'react-i18next';
import { useEffect, useState } from 'react'
import SeatRow from './SeatRow'
import { jsPDF } from "jspdf"
import html2canvas from "html2canvas" // DO NOT REMOVE THIS
import { agendaButtonStyle, headingStyle, linkStyle } from './styles';
import { FaCaretDown, FaCaretUp } from "react-icons/fa";

const chamberStyle = {
    fontSize: "65%",
    height: "50em",
    pageBreakInside: "avoid",
    backgroundColor: "#dedfe1",
    margin: 0,
    padding: 0
}

const chamberPdfStyle = {
    ...chamberStyle,
    backgroundColor: "white",
}

const voteListContainerStyle = {
    columnCount: 3,
    columnGap: "10px",
    boxSizing: "border-box",
    fontSize: "90%"
}

const miniChamberStyle = {
    height: "200px",
    width: "25%",
    pageBreakInside: "avoid",
    backgroundColor: "#dedfe1",
    margin: 0,
    padding: 0,
    alignSelf: "center"
}

const miniChamberPdfStyle = {
    ...miniChamberStyle,
    backgroundColor: "white",
}

const votingInfo = {
    display: "flex",
    flexDirection: "row",
    justifyContent: "space-between",
    width: "90%"
}

const voteLegend = {
    fontWeight: '600',
    width: '2em',
    padding: ' 2px 2px 2px 2px',
    marginBottom: '4px',
    textAlign: 'center',
    boxSizing: 'border-box',
    backgroundColor: "#eeeeee",
    boxSizing: "border-box",
}

const voteCount = {
    paddingTop: "2px",
    paddingBottom: "2px",
    marginBottom: '4px',
    boxSizing: 'border-box',
}

const tableMain = {
    display: "table",
    width: "70%"
}
const tableRow = {
    display: "table-row"
}
const tableCell = {
    display: "table-cell"
}

const seatColorStyles = {
    redDeskStyle: {
        ...voteLegend,
        backgroundColor: "#e62224",
        color: "#ffffff"
    },
    redDeskStyleBW: {
        ...voteLegend,
        backgroundColor: "#aaa",
        color: "#000"
    },
    greenDeskStyle: {
        ...voteLegend,
        backgroundColor: "#64bb46",
        color: "#ffffff"
    },
    greenDeskStyleBW: {
        ...voteLegend,
        backgroundColor: "#000",
        color: "#fff"
    },
    blueDeskStyle: {
        ...voteLegend,
        backgroundColor: "#98d8e1",
        color: "#ffffff"
    },
    blueDeskStyleBW: {
        ...voteLegend,
        backgroundColor: "#fff",
        color: "#000"
    },
    absentStyle: {
        ...voteLegend,
        border: "1px solid black"
    }
}

export default function Voting(props) {
    const [seats, setSeats] = useState([]);
    const [showColors, setShowColors] = useState(true);
    const [seatMap, setSeatMap] = useState([]);
    const [showVotes, setShowVotes] = useState(false)
    const [pdfStyle, setPdfStyle] = useState(false)
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
            let seatId = Number(seat.seatId);
            if (seatId > 100) {
                return
            }

            let voteType = 3;
            let name = seat.person;
            if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
                name += seat.additionalInfoFI?.length > 0 ? ` (${seat.additionalInfoFI})` : ""
            } else {
                name += seat.additionalInfoSV?.length > 0 ? ` (${seat.additionalInfoSV})` : ""
            }

            const vote = voting.votes.find(vote => vote.name === seat.person)
            if (vote !== undefined) {
                vote.name = name
            }

            if (!isNaN(seatId)) {
                tempSeatMap[seatId] = {
                    name,
                    voteType,
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

        setPdfStyle(true)
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
            setPdfStyle(false)
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
            {pdfStyle && (<div style={headingStyle}>{caseNumber}. {title}</div>)}
            <div style={headingStyle}>{t("Voting")}</div>
            <div style={votingInfo}>
                <div style={tableMain}>
                    <div style={tableRow}>
                        <div style={tableCell}>
                            <div style={voteCount}>{getForTitle(voting)}: {voting.forCount}</div>
                            {getForText(voting).length > 0 && <div style={{ marginLeft: "1rem" }}>{t('Proposal')}: <span style={{fontStyle: "italic" }}>{getForText(voting)}</span></div>}
                        </div>
                        <div style={tableCell}>
                            <div style={showColors ? seatColorStyles.greenDeskStyle : seatColorStyles.greenDeskStyleBW}>{voting.forCount}</div>
                        </div>
                    </div>
                    <div style={tableRow}>
                        <div style={tableCell}>
                            <div style={voteCount}>{getAgainstTitle(voting)}: {voting.againstCount}</div>
                            {getAgainstText(voting).length > 0 && <div style={{ marginLeft: "1rem"}}>{t('Proposal')}: <span style={{fontStyle: "italic" }}>{getAgainstText(voting)}</span></div>}
                        </div>
                        <div style={tableCell}>
                            <div style={showColors ? seatColorStyles.redDeskStyle : seatColorStyles.redDeskStyleBW}>{voting.againstCount}</div>
                        </div>
                    </div>
                    <div style={tableRow}>
                        <div style={tableCell}>
                            <div style={voteCount}>{t("Empty")}: {voting.emptyCount}</div>
                        </div>
                        <div style={tableCell}>
                            <div style={showColors ? seatColorStyles.blueDeskStyle : seatColorStyles.blueDeskStyleBW}>{voting.emptyCount}</div>
                        </div>
                    </div>
                    <div style={tableRow}>
                        <div style={tableCell}>
                            <div style={voteCount}>{t("Absent")}: {voting.absentCount}</div>
                        </div>
                        <div style={tableCell}>
                            <div style={seatColorStyles.absentStyle}>{voting.absentCount}</div>
                        </div>
                    </div>

                </div>
                <div style={pdfStyle ? miniChamberPdfStyle : miniChamberStyle}>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={0} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={1} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={2} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={3} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={4} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={5} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={6} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={7} seats={seatMap}></SeatRow>
                    <SeatRow pdfStyle={pdfStyle} showName={false} showColors={showColors} rowNr={8} seats={seatMap}></SeatRow>
                </div>
            </div>

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
                        <div style={pdfStyle ? chamberPdfStyle : chamberStyle}>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={0} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={1} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={2} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={3} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={4} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={5} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={6} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={7} seats={seatMap}></SeatRow>
                            <SeatRow pdfStyle={pdfStyle} showName={true} showColors={showColors} rowNr={8} seats={seatMap}></SeatRow>
                        </div>
                        <p>
                            <a href='javascript:void(0)' onClick={toggleColors} data-html2canvas-ignore={"true"} style={linkStyle}>
                                {showColors ? t("Show black and white vote map") : t("Show vote map with colors")}
                            </a><br /> 
                            <a href="javascript:void(0)" onClick={downloadPDF} data-html2canvas-ignore={"true"} style={linkStyle}>
                                {t("Download voting map pdf")}
                            </a>
                        </p>
                        <div style={voteListContainerStyle} data-html2canvas-ignore={"true"}>
                            <div>{t("FOR")}</div>
                            <br />
                            {voting && voting.votes.filter(vote => vote.voteType === 0).map(vote => createVoterElement(vote))}
                            <br />
                            <div>{t("AGAINST")}</div>
                            <br />
                            {voting && voting.votes.filter(vote => vote.voteType === 1).map(vote => createVoterElement(vote))}
                            <br />
                            <div>{t("EMPTY")}</div>
                            <br />
                            {voting && voting.votes.filter(vote => vote.voteType === 2).map(vote => createVoterElement(vote))}
                            <br />
                            <div>{t("ABSENT")}</div>
                            <br />
                            {voting && voting.votes.filter(vote => vote.voteType === 3).map(vote => createVoterElement(vote))}
                        </div>
                    </div>
                }
            </div>
        </div >
    )
}
