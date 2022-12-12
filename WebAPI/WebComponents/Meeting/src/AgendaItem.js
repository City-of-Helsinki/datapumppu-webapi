
import React, { useEffect } from 'react'
import ReactDOM from 'react-dom'
import parse from 'html-react-parser';


export default function AgendaItem(props) {
    const [accordionOpen, setAccordionOpen] = React.useState(false)
    const agenda = props.agenda
    const index = props.index
    const style = {
        item: {
            flexDirection: "row",
            backgroundColor: accordionOpen ? '#ECECEC' : 'white',
            width: '100%',
            padding: '15px',
            margin: '4px'

        },
        title: {
            fontWeight: 'bold',
        },
        content: {
            paddingTop: '15px'
        },
        icon: {
            fontSize: '10px',
            paddingRight: '10px',
        },
        attachment: {
            paddingBottom: '15px'
        }
    }

    return (
        <div style={style.item}>
            <div role="button" aria-pressed="false" style={style.title} onClick={() => setAccordionOpen(!accordionOpen)}>
                <span style={style.icon} className={
                    accordionOpen
                        ? "glyphicon glyphicon-triangle-top"
                        : "glyphicon glyphicon-triangle-bottom"
                } />
                {index}. {agenda.subject}
            </div>
            {accordionOpen &&
                <div style={style.content} >
                    {agenda.attachments?.map((attachment, index) => {
                        return (
                            <div style={style.attachment} key={'attach'+index}>
                                Liite {index + 1} {''}
                                {attachment.public ?
                                    <a href={attachment.file_uri}>{attachment.name}</a>
                                    : 'Ei-julkinen'}
                            </div>
                        )

                    })}
                    {agenda.content?.map((resolution, index) => {
                        return (
                            <div key={'res'+index}>
                                <div style={style.title}>
                                    {resolution.type}
                                </div>
                                <div>
                                    {parse(resolution.text)}
                                </div>
                            </div>
                        )
                    })}
                </div>
            }
        </div>

    )
}

