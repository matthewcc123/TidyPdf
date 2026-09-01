
export default function Home(){


    return(

      <div className="flex flex-col space-y-3 max-w-2xl mx-auto p-4 text-gray-800">
        <img src="./src/assets/icon_150.png" className="w-12 h-12 self-center" />
        <h2 className="text-md font-semibold self-center text-gray-900">Privacy Policy</h2>
        <p className="text-xs text-gray-500 font-medium self-center mb-2">Effective date: August 26, 2026</p>

        <div className="space-y-4 text-sm text-gray-600">
          <p className="font-medium text-gray-700">
            <strong className="font-semibold text-gray-900">MepLab</strong> ("we," "us," or "Service Provider") operates the <strong className="font-semibold text-gray-900">TidyPdf</strong> application for Windows devices (the "Application").
          </p>

          <p className="font-medium text-gray-700">
            This page informs you of our policies regarding the collection, use, and disclosure of personal data when you use the Application.
          </p>

          <section className="space-y-1">
            <h3 className="text-sm font-semibold text-gray-900">Tracking Data</h3>
            <p>TidyPdf does not collect, log, or store personal information during normal use. Registration is not required.</p>
          </section>

          <section className="space-y-1">
            <h3 className="text-sm font-semibold text-gray-900">Use of Data</h3>
            <p>TidyPdf does not collect personal information through normal use of the Application.</p>
            <p>If you voluntarily provide personal information to MepLab, such as when contacting us directly, that information may be used to respond to your request.</p>
          </section>

          <section className="space-y-1">
            <h3 className="text-sm font-semibold text-gray-900">Transfer Of Data</h3>
            <p>TidyPdf does not share personal information with third parties because the Application does not collect personal information through normal use.</p>
          </section>

          <section className="space-y-1">
            <h3 className="text-sm font-semibold text-gray-900">Security</h3>
            <p>Because the Application does not collect personal data during normal use, the risk of personal data exposure is minimal. However, no security system is completely secure.</p>
            <p>The Service Provider uses reasonable safeguards to protect any information it voluntarily receives.</p>
          </section>

          <section className="space-y-2">
            <h3 className="text-sm font-semibold text-gray-900">Changes To This Privacy Policy</h3>
            <p>The Service Provider may update this Privacy Policy from time to time. Material changes will be communicated by posting the updated Privacy Policy with a new effective date.</p>
            <p>Changes to this Privacy Policy are effective when they are posted on this page, unless otherwise stated.</p>
          </section>

          <section className="pt-2 border-t border-gray-100 space-y-1">
            <h3 className="text-sm font-semibold text-gray-900">Contact Us</h3>
            <p>If you have any questions about this Privacy Policy, please contact us:</p>
            <p>
              <span className="font-medium text-gray-700">By email: </span>
              <a href="mailto:info.meplab@gmail.com" className="text-red-500 hover:underline">info.MepLab@gmail.com</a>
            </p>
          </section>
        </div>
    </div>

    )
}