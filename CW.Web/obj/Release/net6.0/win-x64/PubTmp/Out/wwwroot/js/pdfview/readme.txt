使用方法：
将此js引入到你的网页中，并将pdf修改为你的文档即可
<script src="http://www.lanrenzhijia.com/ajaxjs/pdfobject.js"></script>
<script>
  window.onload = function (){
    var myPDF = new PDFObject({ url: "http://demo.lanrenzhijia.com/2014/pdf1023/sample.pdf" }).embed();
  };
</script>