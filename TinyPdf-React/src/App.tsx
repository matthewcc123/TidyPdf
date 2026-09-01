import { HashRouter as Router, Route, Routes } from "react-router-dom"
import Home from "./pages/Home"
import Privacy from "./pages/Privacy"



function App() {



  return (

    <div className="p-9 min-h-screen min-w-full">
      <div className="layer p-9 max-w-200 w-full flex flex-col space-y-8 justify-self-center">
        <Router>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/privacy" element={<Privacy />} />
          </Routes>
        </Router>
        <p className="mt-8 text-sm text-center text-gray-500">
          Developed by MepLab.
        </p>
      </div>
    </div>

  )
}

export default App
