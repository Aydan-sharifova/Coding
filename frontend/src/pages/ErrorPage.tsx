import { Link, useNavigate } from "react-router-dom";

const content = {
  401:["Authentication required","Sign in to continue to this page."],
  403:["Access denied","You do not have permission to view this resource."],
  404:["Page not found","The page may have moved or the address may be incorrect."],
  500:["Something went wrong","The application encountered an unexpected error. Your data is safe."],
} as const;

export function ErrorPage({ code }: { code:keyof typeof content }) {
  const navigate=useNavigate(); const [title,copy]=content[code];
  return <main className="http-error-page"><span className="brand-mark" aria-hidden="true">C</span><p>{code}</p><h1>{title}</h1><span>{copy}</span><div><button className="ui-button ghost" onClick={()=>navigate(-1)}>Go back</button><Link className="ui-button primary" to={code===401?"/login":"/dashboard"}>{code===401?"Sign in":"Go to dashboard"}</Link></div></main>;
}
